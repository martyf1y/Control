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
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        monsterView = new Vector3(0, 10f, -130); // Change parent variables to the level settings.
        worldView = new Vector3(0, 0, -590);
        rotation = new Vector3(0, 0, 10);
        this.transform.eulerAngles = new Vector3(0, 0, 220);
    }

    // Update is called once per frame
    new void Update()
    {
        
    }
}
