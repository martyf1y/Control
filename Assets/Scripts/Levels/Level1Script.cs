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
    private bool[] buttonPressed; // This tells us if the button has already been pressed.
    private string[] passwordButtLib = { "P", "A", "S", "W", "O", "R", "D" };
    const string password = "ASWORD"; // The Password to get right

    void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        MonsterView = new Vector3(0, 14, -150); // Change parent variables to the level settings.
        WorldView = new Vector3(0, 0, -700);
        Rotation = new Vector3(0, 0, -10);
        this.transform.eulerAngles = new Vector3(0, 0, 120);

        // ---------------- Create the Buttons ---------------- //
        float angleAdjuster = Random.Range(-0.08f, 0.08f);
        float worldEdge = this.transform.localScale.x * -12f; // fixed amount of where the world edge is
        buttText = new TextMeshPro[buttons.Length];
        buttonPressed = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(buttonPrefab, transform) as GameObject;    // Create and set to the parent level 1 with transform     
            
            float angle = angleAdjuster * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, -1);
            buttons[i].transform.position = pos;
            buttons[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 180);
#if DEBUG
            buttText[i] = buttons[i].GetComponentInChildren<TMPro.TextMeshPro>();
#else
            buttText[i].text = passwordButtLib[Random.Range(0, passwordButtLib.Length)];
#endif
            buttText[i].text = passwordButtLib[i];
            buttonPressed[i] = false;
            angleAdjuster -= 0.02f;
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

    public override bool PuzzleChecker(Collider2D characterCollider)
    {
        if (!PuzzleSolved && characterCollider != null)
        {
            int i = 0;
            int totalCorrectButts = 0;

            foreach (GameObject button in buttons)
            {
                ButtonPressCheck(button, i, characterCollider); // Check and change when button pressed
                totalCorrectButts += ButtonCorrectChar(button, i); // Count all correct buttons chars
                i++;
            }
            PuzzleSolved = SolveCheck(totalCorrectButts, password.Length);
             
        }
        return PuzzleSolved;
    }

    void ButtonPressCheck(GameObject thisButton, int index, Collider2D objectCollider)
    { // We check if buttons is pressed and return 1 or 0
        Collider2D buttCollider = thisButton.GetComponent<Collider2D>();
        if (objectCollider.IsTouching(buttCollider))
        {
            if (!buttonPressed[index]) // Have we already pressed?
            {
                int currentCharIndex = System.Array.IndexOf(passwordButtLib, buttText[index].text);
                if (currentCharIndex != passwordButtLib.Length - 1)
                    buttText[index].text = passwordButtLib[currentCharIndex + 1];
                else
                    buttText[index].text = passwordButtLib[0];

                buttonPressed[index] = true;
            }
        }
        else { buttonPressed[index] = false; } // Button can be pressed again
    }

    int ButtonCorrectChar(GameObject thisButton, int index)
    {
        SpriteRenderer buttsprRndr = thisButton.GetComponent<SpriteRenderer>();
        if (buttText[index].text[0] == password[index])
        {
            buttsprRndr.color = Color.green;
            return 1;
        }
        else
        {
            buttsprRndr.color = Color.red;
            return 0;
        }
    }

    bool SolveCheck(int keys, int locks) => Input.GetKey("s") || keys >= locks ? true : false;
}