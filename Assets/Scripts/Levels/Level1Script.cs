using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class Level1Script : World
{
    public static Level1Script instance = null;
    private GameObject[] buttons = new GameObject[6];
    public GameObject buttonPrefab;
    // Our button object properties... even though they are not part of the objects
    TextMeshPro[] buttText;
    private bool[] buttPressed; // This tells us if the button has already been pressed.
    public Sprite[] buttSprite = new Sprite[2];
    private string[] passwordButtLib = { "P", "A", "S", "W", "O", "R", "D" };
    const string password = "ASWORD"; // The Password to get right

    void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        level = 1;
        MonsterView = new Vector3(0, 14, -150); // Change parent variables to the level settings.
        WorldView = new Vector3(0, 0, -700);
        Rotation = new Vector3(0, 0, -10);
        this.transform.eulerAngles = new Vector3(0, 0, 120);

        // ---------------- Create the Buttons ---------------- //
        float angleAdjuster = Random.Range(-0.08f, 0.08f);
        float worldEdge = this.transform.localScale.x * -12.32f; // fixed amount of where the world edge is
        buttText = new TextMeshPro[buttons.Length];
        buttPressed = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(buttonPrefab, transform) as GameObject;    // Create and set to the parent level 1 with transform     

            float angle = angleAdjuster * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, 0);
            buttons[i].transform.position = pos;
            buttons[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 270);
#if DEBUG
            buttText[i] = buttons[i].GetComponentInChildren<TMPro.TextMeshPro>();
#else
            buttText[i].text = passwordButtLib[Random.Range(0, passwordButtLib.Length)];
#endif
            buttText[i].text = passwordButtLib[i];
            buttPressed[i] = false;
            angleAdjuster -= 0.015f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (LevelComplete)
        {
            this.transform.eulerAngles = new Vector3(0, 0, -132.907f);
            FadeOut(); // When level is complete we fade out and remove the level
        }
    }

    public override void PuzzleInteraction(Collider2D monCollider)
    {
        int i = 0;
        foreach (GameObject button in buttons)
        {
            ButtonPressCheck(button, i, monCollider); // Check and change when button pressed
            i++;
        }
    }

    public override bool PuzzleSolvedChecker()
    {
        if (!PuzzleSolved)
        {
            int i = 0;
            int totalCorrectButts = 0;

            foreach (GameObject button in buttons)
            {
                totalCorrectButts += ButtonCorrectChar(button, i); // Count all correct buttons chars
                i++;
            }
            return PuzzleSolved = (Input.GetKeyUp("s") || totalCorrectButts >= password.Length) ? true : false;
        }
        else return PuzzleSolved;
    }

    void ButtonPressCheck(GameObject thisButton, int index, Collider2D objectCollider)
    { // We check if buttons is pressed and return 1 or 0
        Collider2D buttCollider = thisButton.GetComponent<Collider2D>();
        if (objectCollider.IsTouching(buttCollider))
        {
            thisButton.GetComponent<SpriteRenderer>().sprite = buttSprite[1]; // Change to butt down.
            if (!buttPressed[index]) // Have we already pressed?
            {
                int currentCharIndex = System.Array.IndexOf(passwordButtLib, buttText[index].text);
                if (currentCharIndex != passwordButtLib.Length - 1)
                    buttText[index].text = passwordButtLib[currentCharIndex + 1];
                else buttText[index].text = passwordButtLib[0];

                buttPressed[index] = true;
            }
        }
        else
        {
            buttPressed[index] = false;
            thisButton.GetComponent<SpriteRenderer>().sprite = buttSprite[0];
        } 
    }

    int ButtonCorrectChar(GameObject thisButton, int index)
    {
        SpriteRenderer buttsprRndr = thisButton.GetComponent<SpriteRenderer>();
        if (buttText[index].text[0] == password[index])
        {
            buttsprRndr.color = Color.green;
            thisButton.GetComponent<SpriteRenderer>().sprite = buttSprite[1]; // Change to butt down. Has them all go down once puzzle finished.
            return 1;
        }
        else
        {
            buttsprRndr.color = Color.white;
            return 0;
        }
    }
}