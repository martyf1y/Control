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
    public int level = 0;
    public int Level { get{return level;} }
    public Vector3 WorldView { get; set; }
    public Vector3 MonsterView { get; set; }
    public bool PuzzleSolved { get; set; }
    public bool LevelComplete { get; set; }
    public Vector3 Rotation { get; set; }

    void Start()
    {
        LevelComplete = false;
        MonsterView = new Vector3(0, 4, -150);
    }

    public void Rotate(float whatWay)
    {
        transform.Rotate(Rotation * Time.deltaTime * whatWay);
    }

    public virtual void Update() // REMEMBER THIS IS OVERRIDDEN WHEN LEVEL HAS UPDATE
    {
        if (LevelComplete)
            FadeOut();
    }

    public virtual void PuzzleInteraction(Collider2D monCollider)
    {
        // return false;
    }

    public virtual bool PuzzleSolvedChecker()
    {
        return false;
    }

    public virtual void PuzzleSolvedEvents()
    {

    }

    public void FadeOut()
    {
        SpriteRenderer[] allSprites = this.GetComponentsInChildren<SpriteRenderer>();
        Color thisCol;
        foreach (SpriteRenderer thisSprite in allSprites)
        {
            thisCol = thisSprite.color;
            thisCol.a -= fadeSpeed;
            thisSprite.color = thisCol;
        }
        if (this.GetComponent<SpriteRenderer>().color.a <= 0) // Once world is gone, disappear
            this.gameObject.SetActive(false);
    }
}
