using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    public int level = 1;

    public GameObject currentLevel, level0, level1, level2, level3;
    private GameObject worldDoor;

    // General Level Functions
    delegate void MonsterMoveCheck();
    MonsterMoveCheck monsterMoveCheck;

    // Camera shenanigans
    private Vector3 worldViewMain;
    private Vector3 monsterViewMain;
    private Vector3 zoomWhatWay;
    private readonly float camSpeed = .1f;
    private bool movingCam = false;

    // Collision between the objects
    private Collider2D playerCollider, monsterCollider;
    private Collider2D worldCollider, monBlkrCollider;
    private Collider2D doorEntryCollider;

    // Level 2 Variables
    readonly float leashLength = 2.5f;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    void ShortCut() // Debugging purposes
    {
        LevelUp();
        Monster.instance.transform.position = new Vector3(0, 8.539688f, 0);
    }

    void Start()
    {
        //Protection to make sure there is only one instance of GameManager        
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // Cursor.visible = false;

        playerCollider = Player.instance.GetComponent<Collider2D>();
        monsterCollider = Monster.instance.GetComponent<Collider2D>();
        monsterMoveCheck = Lvl1MoveCheck; // Make function the level 1 version

        currentLevel = level1; // Start at level 1
        ResetCurrentWorld(currentLevel);

        transform.position = monsterViewMain; // Start us at right view
#if DEBUG
        // ShortCut();
#endif
    }

    private void Update()
    {
        // Collider bug fix for when animation changes
        if (monsterCollider == null) monsterCollider = Monster.instance.GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = Player.instance.GetComponent<Collider2D>();

        // All levels          
        if (Player.instance.PlayInteract) // Only when player is interacting do checks happen.
        {
            if (PuzzleCheck()) PuzzleSolved();
            if (LevelCompleteCheck()) LevelUp();

            monsterMoveCheck();
        }
        else StopMovement();

        MovingCameraCheck();
    }

    // If moster has collided with door it means they have ccompleted level
    bool LevelCompleteCheck() => (TouchingObjectCheck(worldCollider, monsterCollider, doorEntryCollider) != 'N' ? true : false);

    void LevelUp()
    {
        Player.instance.PlayInteract = false;
        worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
        currentLevel.GetComponentInChildren<Collider2D>().enabled = false; // Break away door trap

        foreach (World world in FindObjectsOfType<World>()) world.Rotation *= -1; // Rotate other way

        if (currentLevel == level1)
        {
            level0.GetComponentInChildren<World>().LevelComplete = true;
            worldDoor.GetComponent<WorldDoor>().ChangeSpriteOrder(2); // Change to behind Monster.
            Monster.instance.Evolve(1);
            Monster.instance.scaleMonster = true; // Bad fix to scale issue between sprites
            monsterMoveCheck = Lvl2MoveCheck;
            Player.instance.movement = Player.instance.Evolve;
            currentLevel = level2; // Move onto level2
        }
        else if (currentLevel == level2)
        {
            level1.GetComponentInChildren<World>().LevelComplete = true;
            currentLevel = level3; // Move onto level3
        }
        ResetCurrentWorld(currentLevel);
        movingCam = true; // Move camera to new view
        zoomWhatWay = monsterViewMain;
    }

    private void Lvl1MoveCheck()
    {
        char monHandDir = TouchingObjectCheck(worldCollider, monsterCollider, playerCollider);
        if (monHandDir != 'N') // When monster on floor and hand is touching we can move monster and worlds
        {
            char monWorldBlkrDir = TouchingObjectCheck(worldCollider, monsterCollider, monBlkrCollider);
            if (monWorldBlkrDir != monHandDir) // Debunk against the level barriers to stop monster going where it cant
            {
                Monster.instance.AnimatePush(monHandDir);
                Player.instance.Flip(monHandDir);
                RotateWorlds(monHandDir);

                Player.instance.AnimatePush();
            }
            else StopMovement();
        }
        else StopMovement();
    }

    void Lvl2MoveCheck()
    {
        Vector3 oldPlayerPos = Player.instance.transform.position;
        Vector3 monsterPos = Monster.instance.transform.position;
        Vector3 newPlayerPos = PullObjectCheck(monsterPos, oldPlayerPos, leashLength); // Calculates player leash limit

        if (oldPlayerPos != newPlayerPos)
        {
            Player.instance.transform.position = newPlayerPos;
            float leashPullDir = Monster.instance.transform.position.x - Player.instance.transform.position.x;

            char whichWayLeash = 'N';
            whichWayLeash = leashPullDir < 0 ? 'R' : 'L';

            if (whichWayLeash != 'N') // When leash is being pulled
            {
                char monWorldBlkrDir = TouchingObjectCheck(worldCollider, monsterCollider, monBlkrCollider);
                if (monWorldBlkrDir == 'N' || monWorldBlkrDir == whichWayLeash) // Debunk against the level barriers to stop monster going where it cant
                {
                    Monster.instance.AnimatePush(whichWayLeash);
                    Player.instance.Flip(whichWayLeash);
                    RotateWorlds(whichWayLeash);

                    Monster.instance.Flip(whichWayLeash);

                    if (Input.GetKey("space") || zoomWhatWay == worldViewMain) // Fast pull
                        Player.instance.AnimatePush();
                    else
                        Player.instance.AnimateStopPush();
                }
                else StopMovement();
            }
        }
        else StopMovement();
    }

    Vector3 PullObjectCheck(Vector3 anchor, Vector3 endVector, float radius)
    {
        Vector3 offset = endVector - anchor;
        // Limits the distance the player can go from the monster with leash
        endVector = anchor + Vector3.ClampMagnitude(offset, radius);
        return endVector;
    }

    public char TouchingObjectCheck(Collider2D groundCol, Collider2D objectCol, Collider2D pusherCol)
    {
        char whichWay = 'N';
        if (objectCol.IsTouching(groundCol) // Is the thing being pushed on the floor?
            && objectCol.IsTouching(pusherCol)) // Is the thing pushed being pushed?
        {
            float posDirection = objectCol.transform.position.x - pusherCol.transform.position.x;
            whichWay = posDirection < 0 ? 'L' : 'R';
        }
        return whichWay;
    }

    bool StopMovement()
    {
        Monster.instance.AnimateStopPush();
        Player.instance.AnimateStopPush();
        return false; // To only run this function once
    }

    void ResetCurrentWorld(GameObject newWorld)
    {
        monsterViewMain = newWorld.GetComponent<World>().MonsterView;
        worldViewMain = newWorld.GetComponent<World>().WorldView;
        worldCollider = newWorld.GetComponent<Collider2D>();
        worldDoor = newWorld.GetComponent<World>().worldDoor;
        doorEntryCollider = newWorld.GetComponent<World>().worldDoor.GetComponent<Collider2D>();
        monBlkrCollider = newWorld.GetComponent<World>().monsterBlocker.GetComponent<Collider2D>();
    }

    bool PuzzleCheck()=> currentLevel.GetComponent<World>().PuzzleChecker(Monster.instance.GetComponent<Collider2D>());

    void PuzzleSolved()
    {
        if (currentLevel == level1)
        { // A one off thing for axe that needed higher access
            float zConv = level0.transform.rotation.z * Mathf.Rad2Deg;
            if (zConv > 52.5 && zConv < 52.8) // Stops axe in the right place
                level0.GetComponentInChildren<World>().Rotation = new Vector3(0, 0, 0);
        }
    }

    void MovingCameraCheck()
    {
        if (Input.GetKeyUp("x"))
        {
            movingCam = true;
            // Decide which way to move camera
            if (transform.position == worldViewMain)
                zoomWhatWay = monsterViewMain;
            else if (transform.position == monsterViewMain)
                zoomWhatWay = worldViewMain;
        }
        if (movingCam) movingCam = MoveCamera(zoomWhatWay);
    }

    bool MoveCamera(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed);
        if (Vector3.Distance(transform.position, targetPos) < .05)
        {
            transform.position = targetPos;
            return false; // We have stopped moving so can stop calling this
        }
        return true;
    }

    void RotateWorlds(char rotateWhatWay)
    {
        float worldSpeed = 1;
        Animator monsterAnimator = Monster.instance.gameObject.GetComponent<Animator>();
        monsterAnimator.speed = 1;

        // Go faster when outer view or holding space bar
        if (Input.GetKey("space") || zoomWhatWay == worldViewMain)
        {
            worldSpeed = 3;
            monsterAnimator.speed = 6;
        }

        foreach (World world in FindObjectsOfType<World>())
            if (!world.LevelComplete) world.Rotate(rotateWhatWay == 'L' ? worldSpeed : -worldSpeed);
    }
}