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
    float leashLength = 2.5f;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    void ShortCut() // Debugging purposes
    {
        worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
        currentLevel.GetComponentInChildren<Collider2D>().enabled = false;
        worldDoor.GetComponent<WorldDoor>().ChangeSpriteOrder(1); // Change to behind Monster.
        level0.GetComponentInChildren<World>().levelComplete = true;
        SpriteRenderer mSprite = Monster.instance.GetComponent<SpriteRenderer>();
        mSprite.sortingOrder += 1;
        Monster.instance.AnimateEvolve();
        Monster.instance.scaleMonster = true; // Bad fix to scale issue between sprites
        Monster.instance.transform.position = new Vector3(0, 8.539688f, 0); 
        monsterMoveCheck = Lvl2MoveCheck;
        Player.instance.movement = Player.instance.Lvl1ToLvl2Transition;
        currentLevel = level2; // Move onto level2
        Player.instance.StopInteract();
        ResetCurrentWorld(currentLevel);
        movingCam = true; // Move camera to new view
        zoomWhatWay = monsterViewMain;

        foreach (World world in FindObjectsOfType<World>()) world.rotation *= -1; // Rotate other way
    }

    // Start is called before the first frame update
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

        ShortCut();
    }

    private void Update()
    {
        // Collider bug fix for when animation changes
        if (monsterCollider==null) monsterCollider = Monster.instance.GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = Player.instance.GetComponent<Collider2D>();

        // All levels          
        if (Player.instance.GetPlayInteract()) // Only when player is interacting do checks happen.
        {
            PuzzleSolvedCheck();
            LvlCompleteCheck();
            monsterMoveCheck();
        }
        else StopMovement();

        MovingCameraCheck();
    }

    private void Lvl1MoveCheck()
    {
        char monHandDir = TouchingObjectCheck(worldCollider, monsterCollider, playerCollider);
        if (monHandDir != 'N') // When monster on floor and hand is touching we can move monster and worlds
        {
            char monWorldBlkrDir = TouchingObjectCheck(worldCollider, monsterCollider, monBlkrCollider);
            if (monWorldBlkrDir != monHandDir) // Debunk against the level barriers to stop monster going where it cant
            {
                Monster.instance.Animate(monHandDir);
                Player.instance.Flip(monHandDir);
                Player.instance.AnimatePush();
                RotateWorlds(monHandDir);
            }
            else StopMovement();
        }
        else StopMovement();
    }

    void Lvl2MoveCheck()
    {
        Vector3 oldPlayerPos = Player.instance.transform.position;
        Vector3 monsterPos = Monster.instance.transform.position;
        Vector3 newPlayerPos = PullObjectCheck(monsterPos, oldPlayerPos, leashLength);

        if (oldPlayerPos != newPlayerPos)
        {
            Player.instance.transform.position = newPlayerPos;
            float leashPullDir = Monster.instance.transform.position.x - Player.instance.transform.position.x;

            char whichWayLeash = 'N';
            whichWayLeash = leashPullDir < 0? 'R' : 'L';

            if (whichWayLeash != 'N') // When monster on floor and hand is touching we can move monster and worlds
            { // ANIMATION TIME!!
                Monster.instance.Flip(whichWayLeash);
                Monster.instance.Animate(whichWayLeash);
                Player.instance.Flip(whichWayLeash);

                if (Input.GetKey("space")) Player.instance.AnimatePush();
                RotateWorlds(whichWayLeash);
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
        Monster.instance.AnimationIdle();
        Player.instance.AnimateStopPush();
        return false; // To only run this function once
    }

    void ResetCurrentWorld(GameObject newWorld)
    {
        monsterViewMain = newWorld.GetComponent<World>().monsterView;
        worldViewMain = newWorld.GetComponent<World>().worldView;
        worldCollider = newWorld.GetComponent<Collider2D>();
        doorEntryCollider = newWorld.GetComponent<World>().worldDoor.GetComponent<Collider2D>();
        monBlkrCollider = newWorld.GetComponent<World>().monsterBlocker.GetComponent<Collider2D>();
        worldDoor = newWorld.GetComponent<World>().worldDoor;
    }

    void LvlCompleteCheck()// ------ THIS IS LEVEL COMPLETE ------ //
    {
        if (TouchingObjectCheck(worldCollider, monsterCollider, doorEntryCollider) != 'N')
        { // We have hit entry to next level.
            worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
            currentLevel.GetComponentInChildren<Collider2D>().enabled = false;
           
            if (currentLevel == level1)
            {
                worldDoor.GetComponent<WorldDoor>().ChangeSpriteOrder(1); // Change to behind Monster.
                level0.GetComponentInChildren<World>().levelComplete = true;
                SpriteRenderer mSprite = Monster.instance.GetComponent<SpriteRenderer>();
                mSprite.sortingOrder += 1;
                Monster.instance.AnimateEvolve();
                Monster.instance.scaleMonster = true; // Bad fix to scale issue between sprites
                monsterMoveCheck = Lvl2MoveCheck;
                Player.instance.movement = Player.instance.Lvl1ToLvl2Transition;     
                currentLevel = level2; // Move onto level2
            }
            else if (currentLevel == level2)
            {
                level1.GetComponentInChildren<World>().levelComplete = true;
                currentLevel = level3; // Move onto level3
            }

            foreach (World world in FindObjectsOfType<World>()) world.rotation *= -1; // Rotate other way

            Player.instance.StopInteract();
            ResetCurrentWorld(currentLevel);
            movingCam = true; // Move camera to new view
            zoomWhatWay = monsterViewMain;
        }
    }

    void PuzzleSolvedCheck()
    {
        if (currentLevel.GetComponentInChildren<World>().puzzleSolved == true)
        {
            if (currentLevel == level1)
            { // A one off thing for axe that needed higher access
                float zConv = level0.transform.rotation.z * Mathf.Rad2Deg;
                if (zConv > 52.5 && zConv < 52.8) // Stops axe in the right place
                    level0.GetComponentInChildren<World>().rotation = new Vector3(0, 0, 0);
            }
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
            if (!world.levelComplete) world.Rotate(rotateWhatWay == 'L' ? worldSpeed : -worldSpeed);
    }
}