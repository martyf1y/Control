using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    public int level = 1;

    //private GameObject currentLevel;
    public GameObject currentLevel, level0, level1, level2, level3;
    private GameObject worldDoor; 
    //public GameObject player;
    //public GameObject monster;

    // If stage 1 is complete then move onto stage 2 things
    // Go up in value of i plus one

    bool puzzleSolved = false;


    // Camera shenanigans
    private Vector3 worldViewMain; 
    private Vector3 monsterViewMain;
    private Vector3 zoomWhatWay;
    private readonly float camSpeed = .1f;
    private bool movingCam = false;

    // How we track collision between the objects
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

        currentLevel = level1;
        ResetCurrentWorld(currentLevel);

        transform.position = monsterViewMain; // Start us at right view
    }

    // Update is called once per frame
    void Update()
    {
        // Level1  
        // Check to see if player is in interaction mode for interaction updates.
        // When monster on floor and hand is touching we can move monster and worlds
        // Debunk against the level barriers to stop monster going where it cant
        // Animate and move worlds based on which way hand is
        // Stop animations when none of the actions are happening
        if (Player.instance.GetPlayInteract()) // There is no point checking most things if player is not doing anything
        {

            PuzzleSolvedCheck();

            LevelCompleteCheck();

            PlayerMoveCheck();



        }
        else // We need to stop things moving
        {
            StopMovement(); // only do once to save running each frame
        }

        // Stops interaction when hand touches world. Kinda annoying
        //if (AreAllObjectsTouching(playerCollider, currentWorldCollider, playerCollider) != 'N') // If player is touuching world stop
        //{
        //    Player.instance.StopInteract();
        //}


        MovingCameraCheck();

        
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


    public char AreAllObjectsTouching(Collider2D groundCol, Collider2D objectCol, Collider2D pusherCol)
    {
        char whichWay = 'N';
       // Debug.Log("collider " + objectCol.IsTouching(pusherCol));
        if (objectCol.IsTouching(groundCol) // Is the thing being pushed on the floor?
            && objectCol.IsTouching(pusherCol)) // Is the thing pushed being pushed?
        { 

            float pusherColPos = pusherCol.transform.position.x;
            float objectColPos = objectCol.transform.position.x;

            if (objectColPos < pusherColPos)
            {
                whichWay = 'L'; // Object is being moved Left
            }
            else
            {
                whichWay = 'R';
            }
        }
        return whichWay;
    }

    void StopMovement()
    {
        Monster.instance.AnimationIdle();
        Player.instance.AnimateStopPush();
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

    void ResetCurrentWorld(GameObject newWorld)
    {
        // currentLevel = newWorld;

        monsterViewMain = newWorld.GetComponent<World>().monsterView;
        worldViewMain = newWorld.GetComponent<World>().worldView;
        worldCollider = newWorld.GetComponent<Collider2D>();
        doorEntryCollider = newWorld.GetComponent<World>().worldDoor.GetComponent<Collider2D>();
        monBlkrCollider = newWorld.GetComponent<World>().monsterBlocker.GetComponent<Collider2D>();
        worldDoor = newWorld.GetComponent<World>().worldDoor;


    }

    void LevelCompleteCheck()
    {
        // ------ THIS IS LEVEL COMPLETE ------ //
        if (AreAllObjectsTouching(worldCollider, monsterCollider, doorEntryCollider) != 'N')
        { // We have hit the entry to next level.
            worldDoor.GetComponent<WorldDoor>().ChangeSprite(1); // Change to open door.
            worldDoor.transform.position = new Vector3(worldDoor.transform.position.x, worldDoor.transform.position.y, -.4f); // Send to behind monster
            currentLevel.GetComponentInChildren<World>().levelComplete = true;

            // todo
            // Change all variables to next level.
            // ResetCurrentWorld(level2);
        }
    }


    void PuzzleSolvedCheck()
    {
        // ------ THIS PUZZLE IS COMPLETE ------ //
        if (currentLevel.GetComponentInChildren<World>().puzzleSolved == true)
        {
            if (currentLevel == level1){ // A one off thing for axe
                level0.GetComponentInChildren<World>().levelComplete = true;
            }

            // This should change to something related to bigger picture. Maybe null until next level reached.
            // monBlkrCollider = level2.GetComponentInChildren<BoxCollider2D>();

        }
    }

    void PlayerMoveCheck()
    {
        // PLEASE TIDY THIS UP SO THE ANIMATIONS CAN CHECK THEIR OWN TRIGGERS       
        char monHandDir = AreAllObjectsTouching(worldCollider, monsterCollider, playerCollider);
        if (monHandDir != 'N') // World, player and monster are touching
        {
            char monWorldBlkrDir = AreAllObjectsTouching(worldCollider, monsterCollider, monBlkrCollider);
            // Debug.Log("collider " + monWorldBlkrDir);
            if (monWorldBlkrDir == monHandDir) // Not touching the blocker
                                               // old way when axe went same was as world --- if (monWorldObjDir == monHandDir && monWorldObjDir != 'N'
                                               //    || monWorldObjDir != monHandDir && monWorldObjDir == 'N') // Checks we are outside boundaries of the world walls
            {
                StopMovement(); 
            }
            else
            {
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

    void MovingCameraCheck()
    {
        if (Input.GetKeyUp("x")){
            movingCam = true;
            // Decide which way to move camera
            if (transform.position == worldViewMain){
                zoomWhatWay = monsterViewMain;
            }
            else if (transform.position == monsterViewMain){
                zoomWhatWay = worldViewMain;
            }
        }
        if (movingCam){
            // Let's move that camera in or out
            movingCam = MoveCamera(zoomWhatWay);
        }
    }
}