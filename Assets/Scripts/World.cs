using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // World properties
    //public Sprite sprite;

    // Fade world
    private int fadeSpeed = 1;
    Color color;

    // Level things properties that cross over each level
    public GameObject worldDoor;
    public Collider2D monsterBlocker;
    public Vector3 worldView  {get; set;}
    public Vector3 monsterView { get; set; }
    public bool puzzleSolved { get; set; }
    public bool levelComplete { get; set; }
    public Vector3 rotation { get; set; }


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
        //Debug.Log(monsterView);
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
