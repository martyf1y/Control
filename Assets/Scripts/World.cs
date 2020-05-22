using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // Fade world
    private const float fadeSpeed = 0.005f;

    // Level things properties that cross over each level
    public GameObject worldDoor;
    public Collider2D monsterBlocker;
    public Vector3 worldView  {get; set;}
    public Vector3 monsterView { get; set; }
    public bool puzzleSolved { get; set; }
    public bool levelComplete { get; set; }
    public Vector3 rotation { get; set; }

    void Start()
    {
        levelComplete = false;
    }

    public void Rotate(float whatWay)
    {
        transform.Rotate(rotation * Time.deltaTime * whatWay);
    }

    public void Update() // REMEMBER THIS IS OVERRIDDEN WHEN LEVEL HAS UPDATE
    {
        if (levelComplete)
            FadeOut();
    }

    public void FadeOut()
    {
        Debug.Log("Goodbye Level");

        SpriteRenderer[] allSprites = this.GetComponentsInChildren<SpriteRenderer>();
        Color thisCol;
        foreach (SpriteRenderer thisSprite in allSprites)
        {
            thisCol = thisSprite.color;
            thisCol.a -= fadeSpeed;
            thisSprite.color = thisCol;
        }
        if(this.GetComponent<SpriteRenderer>().color.a <= 0) // Once world is gone, disappear
            this.gameObject.SetActive(false);
    }
}
