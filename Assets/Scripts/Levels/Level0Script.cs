using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0Script : World
{
    public static Level0Script instance = null;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        rotation = new Vector3(0, 0, 12);
    }


   
}
