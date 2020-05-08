using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    public int level = 1;
    bool level1Complete = false;

    public GameObject level1, level2, level3, level4;
    public GameObject player;
    public GameObject monster;
    public GameObject doorEntry; // The entry point to the next stage



    // Camera shenanigans
    private Vector3 worldView = new Vector3(0, 0.28f, -428.8f);
    private Vector3 monsterView = new Vector3(0, 8, -80);
    private Vector3 zoomWhatWay;
    private readonly float camSpeed = .1f;
    private bool movingCam = false;

    // How we track collision between the objects
    private Collider2D playerCollider, monsterCollider;
    private Collider2D worldCollider, objBlkrCollider;
    private Collider2D doorEntryCollider;
 
    // Start is called before the first frame update
    void Start()
    {   
        if (instance == null) //Protection to make sure there is only one instance of GameManager         
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Cursor.visible = false;

        transform.position = monsterView;
        objBlkrCollider = level1.GetComponentInChildren<BoxCollider2D>();
        worldCollider = level2.GetComponent<Collider2D>();
        playerCollider = player.GetComponent<Collider2D>();
        monsterCollider = monster.GetComponent<Collider2D>();
        doorEntryCollider = doorEntry.GetComponent<Collider2D>();
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

        // PLEASE TIDY THIS UP SO THE ANIMATIONS CAN CHECK THEIR OWN TRIGGERS
        if (Player.instance.GetPlayInteract())
        {
            if (level1Complete == true) // When puzzle complete we take away the barrier
            {
                objBlkrCollider = level3.GetComponentInChildren<BoxCollider2D>();
                
         
            }

            char monHandDir = AreAllObjectsTouching(worldCollider, monsterCollider, playerCollider);
            if (monHandDir != 'N') // World, player and monster are touching
            {


                if (AreAllObjectsTouching(worldCollider, monsterCollider, doorEntryCollider) != 'N')
                { // We have hit the entry to next level.
                    WorldBlocker.instance.changeSprite(1);
                    WorldBlocker.instance.transform.position = new Vector3(WorldBlocker.instance.transform.position.x, WorldBlocker.instance.transform.position.y, -.4f);
                    // todo
                    // Change all variables to next level.
                }


                char monWorldBlkrDir = AreAllObjectsTouching(worldCollider, monsterCollider, objBlkrCollider);
                Debug.Log("collider " + monWorldBlkrDir);
                if (monWorldBlkrDir == monHandDir) // Are we touching the blocker?
                // old way when axe went same was as world --- if (monWorldObjDir == monHandDir && monWorldObjDir != 'N'
                //    || monWorldObjDir != monHandDir && monWorldObjDir == 'N') // Checks we are outside boundaries of the world walls
                {
                    StopMovement(); // only do once to save running each frame
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
                StopMovement(); // only do once to save running each frame
            }

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

        if (Input.GetKeyUp("x"))
        {
            movingCam = true;
            // Decide which way to move camera
            if (transform.position == worldView)
            {
                zoomWhatWay = monsterView;
            }
            else if (transform.position == monsterView)
            {
                zoomWhatWay = worldView;
            }
        }
        if (movingCam)
        {
            // Let's move that camera in or out
            movingCam = MoveCamera(zoomWhatWay);
        }
    }

    void RotateWorlds(char rotateWhatWay)
    {
        float worldSpeed = 1;
        Animator monsterAnimator = monster.gameObject.GetComponent<Animator>();

        // Go faster when outer view or holding space bar
        if (Input.GetKey("space") || zoomWhatWay == worldView)
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
            world.Rotate(rotateWhatWay == 'L' ? worldSpeed : -worldSpeed);
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
}