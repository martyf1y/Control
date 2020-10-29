using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    Monster monster;
    Player player;
    private GameObject currentWorld;
    [SerializeField] private GameObject level0, level1, level2, level3;
    private GameObject worldDoor;

    // General Level Functions
    delegate bool PuzzleSolved();
    PuzzleSolved isPuzzleSolved;
    delegate void WorldEvents();
    WorldEvents worldInteraction, worldCompleted;

    // Collider Functions
    delegate char DirectionCheck();
    DirectionCheck moveDirection, barrierDirection;

    // Camera shenanigans
    private Vector3 worldView, monsterView, targetView;
    private readonly float camSpeed = .1f;

    // Collision between objects
    private Collider2D playerCol, monCol, worldCol, barrierCol, doorCol;

    // Level 2 Variables
    const float leashLength = 2.5f;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    void ShortCut() // Debugging purposes
    {
        LevelUp();
        monster.transform.position = new Vector3(0, 8.490003f, 0);
    }

    void Start()
    {
        Cursor.visible = false;
        if (instance == null) instance = this; // Protection to make sure there is only one instance of GameManager        
        else if (instance != this) Destroy(this);
        monster = Monster.instance;
        player = Player.instance;


        moveDirection = L1PushMonsterCheck; // Make function the level 1 version
        barrierDirection = L1BarrierCheck;
        worldInteraction = L1PuzzleInteraction;
        currentWorld = level1; // Start at level 1
        ResetToWorld(currentWorld);
        worldCompleted = level0.GetComponent<World>().PuzzleSolvedEvents;
        transform.position = monsterView; // Start us at right view

#if DEBUG
        ShortCut();
#endif
    }

    private void Update()
    {
        // Collider bug fix for when animation changes
        if (monCol == null) monCol = monster.GetComponent<Collider2D>();
        if (playerCol == null) playerCol = player.GetComponent<Collider2D>();

        char direction = 'N';
        if (player.Interact) // Only when player is interacting do checks happen.
        {
            if (targetView == worldView) player.moveSpeed = .02f;
            else player.moveSpeed = .08f;

            direction = moveDirection(); // Based on current world factors
            if (direction == barrierDirection()) direction = 'N';
            if (direction == 'N' && Input.GetKeyDown("space")) player.HandSlam();
        }

        if (direction != 'N')
        {
            player.move(direction);
            monster.move(direction);
            RotateWorlds(direction);
        }
        else IdleAnimation();

        if (!UpdatingCamera(targetView) && Input.GetKeyUp("x")) ChangeCameraView();

        if (!isPuzzleSolved()) worldInteraction();
        else worldCompleted();
        if (IsLevelCompleted) LevelUp();
    }

    private char L1PushMonsterCheck() => GetObjectDirection(monCol, playerCol, worldCol);

    private char L2PullMonsterCheck()
    {
        Vector3 oldPlayerPos = player.transform.position;
        Vector3 monsterPos = monster.transform.position;
        Vector3 pullLimit = PullObjectLimit(monsterPos, oldPlayerPos, leashLength);
        if (oldPlayerPos != pullLimit)
        {
            player.transform.position = pullLimit;
            // Player cannot pull if touching ground
            if (!playerCol.IsTouching(worldCol)) return monsterPos.x < pullLimit.x ? 'R' : 'L';
        }
        return 'N';
    }

    char L1BarrierCheck() => GetObjectDirection(monCol, barrierCol, worldCol);
    char L2BarrierCheck() => GetObjectDirection(playerCol, barrierCol);

    private void L1PuzzleInteraction() => currentWorld.GetComponent<World>().PuzzleInteraction(monCol);

    private void L2PuzzleInteraction()
    {
        Color dogColor = monster.GetComponent<SpriteRenderer>().color;
        Level2Script L2 = currentWorld.GetComponent<Level2Script>();

        float slamSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude;
        if (monster.HitOnHead(playerCol, slamSpeed) && player.Interact)
        {
            player.Force = 0;
            if (dogColor == Color.white) // Check to see if dog is already holding paper
            {
                dogColor = L2.PickupPaper(monCol, dogColor);
                if (dogColor != Color.white) monster.GrabNewspaper();
            }
            else
            {
                Color returnedMailColor = L2.DropPaper(monCol, dogColor);
                if (returnedMailColor != Color.white) L2.TurnOnLight(returnedMailColor);
                dogColor = Color.white;
                monster.DropNewspaper(); // Change to dog without paper
            }
            monster.GetComponent<SpriteRenderer>().color = dogColor;
        }
        if (monster.GetComponent<Animator>().GetBool("WithPaperB"))
            L2.UpdatePickedUpPaper(monCol, dogColor);
    }


    void IdleAnimation()
    {
        monster.StopPush();
        player.StopPush();
    }

    // ------- Collider Checks Changes ------ //

    Vector3 PullObjectLimit(Vector3 anchor, Vector3 pullVector, float radius)
    {
        Vector3 offset = pullVector - anchor;
        pullVector = anchor + Vector3.ClampMagnitude(offset, radius);
        return pullVector;
    }

    public char GetObjectDirection(Collider2D obj, Collider2D pusher, Collider2D ground) =>
         obj.IsTouching(ground) // Is object on the floor?
         && obj.IsTouching(pusher)
            ? obj.transform.position.x < pusher.transform.position.x
                ? 'L' : 'R' : 'N';

    public char GetObjectDirection(Collider2D obj, Collider2D pusher) => // SHOULD BE WORKING BUT IS NOT? MIGHT BE RELATED TO LINKING ISSUE WITH SCRIPTS
         obj.IsTouching(pusher) ? // Is object being pushed?
            obj.transform.position.x < pusher.transform.position.x ?
                'R' : 'L' : 'N';


    // If monster has collided with door then level complete
    private bool IsLevelCompleted => GetObjectDirection(monCol, doorCol, worldCol) != 'N' ? true : false;

    // ------- World Changes ------ //
    void ResetToWorld(GameObject level)
    {
        World newWorld = level.GetComponent<World>();
        monsterView = newWorld.MonsterView;
        worldView = newWorld.WorldView;
        worldCol = level.GetComponent<Collider2D>();
        worldDoor = newWorld.worldDoor;
        doorCol = newWorld.worldDoor.GetComponent<Collider2D>();
        barrierCol = newWorld.monsterBlocker.GetComponent<Collider2D>();
        isPuzzleSolved = newWorld.PuzzleSolvedChecker;
        worldCompleted = newWorld.PuzzleSolvedEvents;
        targetView = monsterView;
        player.UpdateMove(newWorld.Level);
        monster.UpdateMove(newWorld.Level);
    }

    

    void LevelUp()
    {
        player.Interact = false; // Stops player moving
        worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
        currentWorld.GetComponentInChildren<Collider2D>().enabled = false; // Remove door blocker

        foreach (World world in FindObjectsOfType<World>()) world.Rotation *= -1; // Rotate other way

        if (currentWorld == level1)
        {
            level0.GetComponentInChildren<World>().LevelComplete = true;
            worldDoor.GetComponent<WorldDoor>().ChangeSpriteOrder("Back Object"); // Change to behind Monster.
            worldDoor.transform.GetChild(0).gameObject.SetActive(false); // Mask

            moveDirection = L2PullMonsterCheck;
            barrierDirection = L2BarrierCheck;
            worldInteraction = L2PuzzleInteraction;

            monster.Evolve(1);
            player.movement = player.Evolve;
            player.attachCollarHere = monster.transform.Find("Collar").transform;

            currentWorld = level2;
        }
        else if (currentWorld == level2)
        {
            level1.GetComponentInChildren<World>().LevelComplete = true;
            currentWorld = level3;
        }
        ResetToWorld(currentWorld);
    }

    void ChangeCameraView()
    {
        if (transform.position == worldView) targetView = monsterView;
        else if (transform.position == monsterView) targetView = worldView;
    }

    bool UpdatingCamera(Vector3 tPos)
    {
        if (Vector3.Distance(transform.position, tPos) > .05)
        {
            transform.position = Vector3.Lerp(transform.position, tPos, camSpeed);
            return true;
        }

        transform.position = tPos;
        return false; // We have stopped moving so can stop calling this
    }

    void RotateWorlds(char dir)
    {
        float speed = 1;
        Animator monsterAnimator = monster.GetComponent<Animator>();
        monsterAnimator.speed = 1;
        if (Input.GetKey("space") || targetView == worldView) // Go faster when outer view or holding space bar
        {
            speed = 3;
            monsterAnimator.speed = 6;
        }
        foreach (World world in FindObjectsOfType<World>())
            if (!world.LevelComplete) world.Rotate(dir == 'L' ? speed : -speed);
    }
}