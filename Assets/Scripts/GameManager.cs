using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
                                               // private int level = 1;

    private GameObject currentLevel;
    [SerializeField] private GameObject level0 = null;
    [SerializeField] private GameObject level1 = null;
    [SerializeField] private GameObject level2 = null;
    [SerializeField] private GameObject level3 = null;
    private GameObject worldDoor;

    // General Level Functions
    delegate bool WorldPuzzleCheck();
    WorldPuzzleCheck IsWorldPuzzleSolved;
    delegate void WorldInteraction();
    WorldInteraction WorldPreSolvedInteractions;
    // Monster Functions
    delegate char MonsterMoveCheck();
    MonsterMoveCheck GetMonMoveDir, BarrierCheck;
    delegate void MoveMonsterType(char direction);
    MoveMonsterType MoveMonAnim;

    // Camera shenanigans
    private Vector3 worldViewCoordinates;
    private Vector3 monViewCoordinates;
    private Vector3 newViewCoordinates;
    private readonly float camSpeed = .1f;
    private bool movingCam = false;

    // Collision between the objects
    private Collider2D playerCollider, monCollider;
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
        Monster.instance.transform.position = new Vector3(0, 8.490003f, 0);
    }

    void Start()
    {
        //Protection to make sure there is only one instance of GameManager        
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

         Cursor.visible = false;

        playerCollider = Player.instance.GetComponent<Collider2D>();
        monCollider = Monster.instance.GetComponent<Collider2D>();
        GetMonMoveDir = Lvl1PlayerPushMonCheck; // Make function the level 1 version
        BarrierCheck = Lvl1BarrierCheck;
        MoveMonAnim = Lvl1MoveMon;
        WorldPreSolvedInteractions = Lvl1PuzzleInteraction;
        currentLevel = level1; // Start at level 1
        ResetCurrentWorld(currentLevel);


        transform.position = monViewCoordinates; // Start us at right view
#if DEBUG
       // ShortCut();
#endif
    }

    private void Update()
    {
        // Collider bug fix for when animation changes
        if (monCollider == null) monCollider = Monster.instance.GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = Player.instance.GetComponent<Collider2D>();

        if (Player.instance.PlayInteract) // Only when player is interacting do checks happen.
        {
            char monMoveDirection = GetMonMoveDir(); // Based on current world factors

            if (monMoveDirection != 'N' && monMoveDirection != BarrierCheck())
            {
                MoveMonAnim(monMoveDirection);
                RotateWorlds(monMoveDirection);
            }
            else if (Input.GetKeyDown("space")) Player.instance.HitAttack(); // Hand slam
            else StopMoveAnim();

            if (!IsWorldPuzzleSolved())
                WorldPreSolvedInteractions();
            else
                WorldPuzzleSolvedEvents();

            if (IsLevelCompleted)
            {
                Player.instance.PlayInteract = false; // Stops player moving
                LevelUp();
            }
        }
        else StopMoveAnim();

        if (movingCam || WantToMoveCameraCheck()) movingCam = MoveCamera(newViewCoordinates);
    }

    private void Lvl1MoveMon(char monsterDir)
    {
        Monster.instance.AnimatePush(monsterDir);
        Player.instance.Flip(monsterDir);
        Player.instance.AnimatePush();
    }

    private void Lvl2MoveMon(char monsterDir)
    {
        Monster.instance.AnimatePush(monsterDir);
        Player.instance.Flip(monsterDir);
        Monster.instance.Flip(monsterDir);

        if (Input.GetKey("space") || newViewCoordinates == worldViewCoordinates) // Fast pull
        {
            Player.instance.AnimatePush();
            Player.instance.GetComponentInChildren<Rope>().CheckRopeAdjust(Rope.maxLength - 5);
        }
        else
        {
            Player.instance.AnimateStopPush();
            Player.instance.GetComponentInChildren<Rope>().CheckRopeAdjust(Rope.maxLength);
        }
    }

    private char Lvl1PlayerPushMonCheck() => TriObjectCollCheck(worldCollider, monCollider, playerCollider);

    private char Lvl2PlayerPullMonCheck()
    {
        Vector3 oldPlayerPos = Player.instance.transform.position;
        Vector3 monsterPos = Monster.instance.transform.position;
        Vector3 playerPullLimit = PullObjectLimitCalc(monsterPos, oldPlayerPos, leashLength); // Calculates player leash limit and place
        if (oldPlayerPos != playerPullLimit)
        {
            Player.instance.transform.position = playerPullLimit;
            if (!playerCollider.IsTouching(worldCollider)) // Player cannot pull if touching ground
                return monsterPos.x < playerPullLimit.x ? 'R' : 'L';
        }
        return 'N';
    }

    char Lvl1BarrierCheck() => TriObjectCollCheck(worldCollider, monCollider, monBlkrCollider);

    char Lvl2BarrierCheck() => DoubObjectCollCheck(playerCollider, monBlkrCollider);

    private void Lvl1PuzzleInteraction() => currentLevel.GetComponent<World>().PuzzleInteraction(monCollider);

    private void Lvl2PuzzleInteraction()
    {
        SpriteRenderer monColor = Monster.instance.GetComponent<SpriteRenderer>();
        if (monColor.color != Color.white)
            currentLevel.GetComponent<Level2Script>().UpdatePickedUpPaper(monCollider, monColor.color);

        if (HandHitDogFromAbove(monCollider, playerCollider, Player.instance.GetComponent<Rigidbody2D>().velocity.magnitude))
        {
            Player.instance.Force = 0;
            if (monColor.color == Color.white) // Check to see if dog is already holding paper
            {
                monColor.color = currentLevel.GetComponent<Level2Script>().PickupPaper(monCollider, monColor.color);
                if (monColor.color != Color.white) // If the dog picked up a paper 
                    Monster.instance.AnimateGrabNewspaper();
            }
            else
            {
                if (currentLevel.GetComponent<Level2Script>().PaperDropped(monCollider, monColor.color) != null)
                {
                    // Switch on light in house
                }
                monColor.color = Color.white;
                Monster.instance.AnimateDropNewspaper(); // Change to dog without paper
            }
        }
    }

    private bool HandHitDogFromAbove(Collider2D mTransform, Collider2D pTransform, float speed) =>
        playerCollider.IsTouching(monCollider) &&
        pTransform.transform.position.y > mTransform.transform.position.y &&
        ((mTransform.transform.localScale.x == -1 && pTransform.transform.position.x < mTransform.transform.position.x) ||
        (mTransform.transform.localScale.x == 1 && pTransform.transform.position.x > mTransform.transform.position.x)) &&
        speed > 1f ? true : false; // Is player hitting dog from above?

    void StopMoveAnim()
    {
        Monster.instance.AnimateStopPush();
        Player.instance.AnimateStopPush();
    }

    // ------- Collider Checks Changes ------ //

    Vector3 PullObjectLimitCalc(Vector3 anchor, Vector3 pullVector, float radius)
    {
        Vector3 offset = pullVector - anchor;
        // Limits the distance the player can go from the monster with leash
        pullVector = anchor + Vector3.ClampMagnitude(offset, radius);
        return pullVector;
    }

    public char TriObjectCollCheck(Collider2D groundCol, Collider2D objectCol, Collider2D pusherCol) =>
         objectCol.IsTouching(groundCol) // Is object on the floor?
         && objectCol.IsTouching(pusherCol)
            ? objectCol.transform.position.x < pusherCol.transform.position.x
                ? 'L' : 'R' : 'N';

    public char DoubObjectCollCheck(Collider2D objectCol, Collider2D pusherCol) => // SHOULD BE WORKING BUT IS NOT? MIGHT BE RELATED TO LINKING ISSUE WITH SCRIPTS
         objectCol.IsTouching(pusherCol) ? // Is object being pushed?
            objectCol.transform.position.x < pusherCol.transform.position.x ?
                'R' : 'L' : 'N';


    // If moster has collided with door it means they have ccompleted level
    private bool IsLevelCompleted => TriObjectCollCheck(worldCollider, monCollider, doorEntryCollider) != 'N' ? true : false;

    // ------- World Changes ------ //

    void ResetCurrentWorld(GameObject newWorld)
    {
        monViewCoordinates = newWorld.GetComponent<World>().MonsterView;
        worldViewCoordinates = newWorld.GetComponent<World>().WorldView;
        worldCollider = newWorld.GetComponent<Collider2D>();
        worldDoor = newWorld.GetComponent<World>().worldDoor;
        doorEntryCollider = newWorld.GetComponent<World>().worldDoor.GetComponent<Collider2D>();
        monBlkrCollider = newWorld.GetComponent<World>().monsterBlocker.GetComponent<Collider2D>();
        IsWorldPuzzleSolved = currentLevel.GetComponent<World>().PuzzleSolvedChecker;
        movingCam = true; // Move camera to new view
        newViewCoordinates = monViewCoordinates;
    }

    void WorldPuzzleSolvedEvents()
    {
        if (currentLevel == level1)
        { // A one off thing for axe that needed higher access
            float zConv = level0.transform.rotation.z * Mathf.Rad2Deg;
            if (zConv > 52.5 && zConv < 52.8) // Stops axe in the right place
                level0.GetComponentInChildren<World>().Rotation = new Vector3(0, 0, 0);
        }
    }

    void LevelUp()
    {
        worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
        currentLevel.GetComponentInChildren<Collider2D>().enabled = false; // Remove door blocker

        foreach (World world in FindObjectsOfType<World>()) world.Rotation *= -1; // Rotate other way

        if (currentLevel == level1)
        {
            level0.GetComponentInChildren<World>().LevelComplete = true;
            worldDoor.GetComponent<WorldDoor>().ChangeSpriteOrder("Back Object"); // Change to behind Monster.
            worldDoor.transform.GetChild(0).gameObject.SetActive(false); // Mask

            Monster.instance.Evolve(1);
            GetMonMoveDir = Lvl2PlayerPullMonCheck;
            MoveMonAnim = Lvl2MoveMon;
            BarrierCheck = Lvl2BarrierCheck;
            WorldPreSolvedInteractions = Lvl2PuzzleInteraction;
            Player.instance.movement = Player.instance.Evolve;
            Player.instance.attachCollarHere = Monster.instance.transform.Find("Collar").transform;
            currentLevel = level2; // Move onto level2
        }
        else if (currentLevel == level2)
        {
            level1.GetComponentInChildren<World>().LevelComplete = true;
            currentLevel = level3; // Move onto level3
        }
        ResetCurrentWorld(currentLevel);
    }

    bool WantToMoveCameraCheck()
    {
        if (Input.GetKeyUp("x"))
        {
            // Decide which way to move camera
            if (transform.position == worldViewCoordinates)
                newViewCoordinates = monViewCoordinates;
            else if (transform.position == monViewCoordinates)
                newViewCoordinates = worldViewCoordinates;
            return true;
        }
        return false;
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
        Animator monsterAnimator = Monster.instance.GetComponent<Animator>();
        monsterAnimator.speed = 1;

        // Go faster when outer view or holding space bar
        if (Input.GetKey("space") || newViewCoordinates == worldViewCoordinates)
        {
            worldSpeed = 3;
            monsterAnimator.speed = 6;
        }

        foreach (World world in FindObjectsOfType<World>())
            if (!world.LevelComplete) world.Rotate(rotateWhatWay == 'L' ? worldSpeed : -worldSpeed);
    }
}