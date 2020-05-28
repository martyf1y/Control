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

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) //Protection to make sure there is only one instance of GameManager         
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Cursor.visible = false;

        playerCollider = Player.instance.GetComponent<Collider2D>();
        monsterCollider = Monster.instance.GetComponent<Collider2D>();

        currentLevel = level1; // Start at level 1
        ResetCurrentWorld(currentLevel);
        monsterMoveCheck = Lvl1MoveCheck; // Make function the level 1 version

        transform.position = monsterViewMain; // Start us at right view
    }

    void Update()
    {

        if (monsterCollider==null) { // Bug fix for when monster animate changes
            monsterCollider = Monster.instance.GetComponent<Collider2D>();
        }


        // All levels          
        if (Player.instance.GetPlayInteract()) // Check to see if player is in interaction mode for interaction updates.
        {
            PuzzleSolvedCheck();

            LvlCompleteCheck();

            monsterMoveCheck();
        }
        else // We need to stop things moving
        {
            StopMovement(); // only do once to save running each frame
        }

        MovingCameraCheck();
    }

    void Lvl1MoveCheck()
    {
        char monHandDir = TouchingObjectCheck(worldCollider, monsterCollider, playerCollider);
        if (monHandDir != 'N') // When monster on floor and hand is touching we can move monster and worlds
        {
            char monWorldBlkrDir = TouchingObjectCheck(worldCollider, monsterCollider, monBlkrCollider);
            if (monWorldBlkrDir == monHandDir) // Debunk against the level barriers to stop monster going where it cant
            {   // Stop animations when none of the actions are happening
                StopMovement();
            }
            else
            {   // Animate and move worlds based on which way hand is interacting from
                Monster.instance.Animate(monHandDir);
                Player.instance.Flip(monHandDir);
                Player.instance.AnimatePush();
                RotateWorlds(monHandDir);
            }
        }
        else
        {
            StopMovement();
        }
    }
    void Lvl2MoveCheck()
    {
        Vector3 oldPlayerPos = Player.instance.transform.position;
        Vector3 monsterPos = Monster.instance.transform.position;
        Vector3 newPlayerPos = PullObjectCheck(monsterPos, oldPlayerPos, 4);

        if (oldPlayerPos != newPlayerPos)
        {
            Player.instance.transform.position = newPlayerPos;

            Debug.Log("PULL DOG");

            float leashPullDir = Monster.instance.transform.position.x - Player.instance.transform.position.x;

            char whichWayLeash = 'N';
                if (leashPullDir < 0)
            {
                whichWayLeash = 'R';
            }
            else
            {
                whichWayLeash = 'L';
            }
            // ANIMATION TIME!!
            Monster.instance.Flip(whichWayLeash);
            Monster.instance.Animate(whichWayLeash);
            Player.instance.Flip(whichWayLeash);
            Player.instance.AnimatePush();
            RotateWorlds(whichWayLeash);

        }
        else
        {
            StopMovement();
        }
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

            if (posDirection < 0)
                whichWay = 'L'; // Object is being moved Left
            else
                whichWay = 'R';
        }
        return whichWay;
    }

    void StopMovement()
    {
        Monster.instance.AnimationIdle();
        Player.instance.AnimateStopPush();
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
            currentLevel.GetComponentInChildren<World>().levelComplete = true;
            currentLevel.GetComponentInChildren<Collider2D>().enabled = false;

            if (currentLevel == level1)
            { 
                SpriteRenderer mSprite = Monster.instance.GetComponent<SpriteRenderer>();
                mSprite.sortingOrder += 1;
                Monster.instance.AnimateEvolve();
                monsterMoveCheck = Lvl2MoveCheck;
                Player.instance.movement = Player.instance.Lvl1ToLvl2Transition;
                var worlds = FindObjectsOfType<World>();
                foreach (var world in worlds)
                {
                    world.rotation *= -1; // Time to go the other way
                }
                currentLevel = level2; // Move onto level2
            }
            else if (currentLevel == level2)
            {
                currentLevel = level3; // Move onto level3
            }

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
                Debug.Log(zConv);
                if (zConv > 52.5 && zConv < 52.8)
                { // Stops axe in the right place
                    level0.GetComponentInChildren<World>().levelComplete = true;
                    // level0.transform.rotation.z = 236.579;
                }
            }

            // todo
            // Level blocker becomes deactivated

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
        if (movingCam)
            movingCam = MoveCamera(zoomWhatWay);
    }

    bool MoveCamera(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed);

        if (Vector3.Distance(transform.position, targetPos) < .05)
        {
            transform.position = targetPos;
            return false; // We have stopped moving so can stop calling this
        }
        else
        {
            return true;
        }
    }

    void RotateWorlds(char rotateWhatWay)
    {
        float worldSpeed = 1;
        Animator monsterAnimator = Monster.instance.gameObject.GetComponent<Animator>();

        // Go faster when outer view or holding space bar
        if (Input.GetKey("space") || zoomWhatWay == worldViewMain)
        {
            worldSpeed = 3;
            monsterAnimator.speed = 6;
        }
        else
        {
            monsterAnimator.speed = 1;
        }
        // Debug.Log(toMove);
        var worlds = FindObjectsOfType<World>();
        foreach (var world in worlds)
        {
            // Depending on which way we are moving we rotate the world
            if (!world.levelComplete) // We stop moving once level complete
            {
                world.Rotate(rotateWhatWay == 'L' ? worldSpeed : -worldSpeed);
            }
        }
    }
}