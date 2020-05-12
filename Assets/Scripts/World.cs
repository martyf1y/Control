using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // World properties
    //public Sprite sprite;
    [SerializeField]
    private Vector3 rotation = new Vector3(0,0,1);
    private int fadeSpeed = 1;
    Color color;

    // Level things properties that cross over each level
    bool levelComplete = false;
    public Vector3 worldView; // = new Vector3(0, 0, -450); // ()
    public Vector3 monsterView; // = new Vector3(0, 8.5f, -80); // ()
    public GameObject worldDoor;
    public Collider2D monsterBlocker;


    // Start is called before the first frame update
    void Start()
    {
        color = this.GetComponent<SpriteRenderer>().material.color;
    }

    // Update is called once per frame
    public void Rotate(float whatWay)
    {
        transform.Rotate(rotation * Time.deltaTime * whatWay);

    }

    public void Update()
    {
        Debug.Log(monsterView);
    }

    public void FadeOut()
    {
        color.a -= Time.deltaTime * fadeSpeed;
        this.GetComponent<SpriteRenderer>().material.color = color;
        if(color.a <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
