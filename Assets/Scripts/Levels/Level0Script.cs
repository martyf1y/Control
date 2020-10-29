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

        Rotation = new Vector3(0, 0, 12);
        this.transform.eulerAngles = new Vector3(0, 0, 232.907f); // 172

    }

    public override void PuzzleSolvedEvents()
    {
        float zConv = transform.rotation.z * Mathf.Rad2Deg;
        if (zConv > 52.5 && zConv < 52.8) // Stops axe in the right place
            GetComponentInChildren<World>().Rotation = new Vector3(0, 0, 0);
    }
}
