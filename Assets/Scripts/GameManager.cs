using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public int level = 1;


    public GameObject level2, level3, level4;
    public GameObject player;
    Collider2D playerCol, currentWorldCol;



    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

         Cursor.visible = false;

        currentWorldCol = level2.GetComponent<Collider2D>();
        playerCol = player.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // case level1  
        /// If player and monster are colliding and mousedown
        /// Player is pushing from left
        /// If player is pushing on the right of monster
        /// if player is pushing on the left
        /// 
        // When the monster moves we move the world.
       

        
            CheckRotation();
        

    }


    void CheckRotation()
    {
        bool playerInteracting = Player.instance.GetPlayInteract();
        
        char toMove = Monster.instance.DoWeMove(currentWorldCol, playerCol, playerInteracting);

        if (toMove != 'N') // Change this to an event check
        {
            float worldSpeed = 1;
            if (Input.GetKey("space"))
                worldSpeed = 3;
            // Debug.Log(toMove);
            var worlds = FindObjectsOfType<World>();
            foreach (var world in worlds)
            {
                world.Rotate(toMove == 'L' ? worldSpeed : -worldSpeed);
                //  world.FadeOut();
            }
        }

    }

}