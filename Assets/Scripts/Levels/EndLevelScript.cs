using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelScript : MonoBehaviour
{
    public static EndLevelScript instance = null;
    private 

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        this.transform.eulerAngles = new Vector3(0, 0, 236.579f);

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Level0Script.instance.transform.rotation;
    }
}
