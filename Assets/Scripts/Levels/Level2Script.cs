using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Script : World
{
    public static Level2Script instance = null;

    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);


       // monsterView = new Vector3(0, 8.5f, -80); // Change parent variables to the level settings.
       // worldView = new Vector3(0, 0, -450);
        rotation = new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
