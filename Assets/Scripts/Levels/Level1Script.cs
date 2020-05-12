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


    // Declare the objects for the puzzle
    // 

    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);



    }

    // Update is called once per frame
    new void Update()
    {

    }

    void PuzzleAction(GameObject other) // This will be where the program does the specific puzzle check
    {
        

    }
}
