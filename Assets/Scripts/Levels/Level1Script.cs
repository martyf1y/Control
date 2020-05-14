using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Script : World
{
    public static Level1Script instance = null;
    /// <summary>
    /// Create new gameobjects based on the to be made button class
    /// Have a button check that takes monster collider
    /// Trigger a change eveythime collider hits
    /// Run cheker in manager
    /// // Change the gameobject to the new one and call the functions when needed in the sections that are relevant for it.

    /// world1.update
    /// </summary>

    //public GameObject prefabButton;
    private GameObject[] buttons;
    private float posBasedOnWorld;


    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);


        monsterView = new Vector3(0, 8.5f, -80); // Change parent variables to the level settings.
        worldView = new Vector3(0, 0, -450);
        rotation = new Vector3(0, 0, -10);

        buttons = GameObject.FindGameObjectsWithTag("Button");


        // ADD IN BUTTON POSITION HERE BASED ON WORLD SCALE
        posBasedOnWorld = this.transform.localScale.x * 7.4f; // fixed amount of where the world edge is


    }

    // Update is called once per frame
    new void Update()
    {
        //Debug.Log("Scale " + this.transform.localScale);

        int i = 0;
        Collider2D monsterCollider = Monster.instance.GetComponent<Collider2D>();
        foreach (var button in buttons)
        {
            Collider2D buttCollider = button.GetComponent<Collider2D>();
            if (monsterCollider.IsTouching(buttCollider))
            {
                Debug.Log("Touching!! " + i + " " + button.transform.position);
            }
            i++;
        }

        if (Input.GetKey("s")) // or if all buttons are correct
        {
            puzzleSolved = true;
        }
        //PuzzleAction();
    }

    public void ChangeLetter(int num)
    {
        //  WBSpriteRenderer.sprite = WBSprite[num];
    }

    void PuzzleAction(GameObject other) // This will be where the program does the specific puzzle check
    {


    }
}
