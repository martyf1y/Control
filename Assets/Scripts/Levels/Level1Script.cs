﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1Script : World
{
    public static Level1Script instance = null;

    private GameObject[] buttons;
    // Our button object properties... even though they are not part of the objects
    TextMeshPro[] buttText;
    bool[] buttonPressed; // This tells us if the button has already been pressed.
    private string[] passwordButtLib = { "P", "A", "S", "W", "O", "R", "D" };
    const string password = "ASWORD"; // The Password to get right

    void Start()
    {
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            Destroy(gameObject);

        monsterView = new Vector3(0, 8.5f, -80); // Change parent variables to the level settings.
        worldView = new Vector3(0, 0, -450);
        rotation = new Vector3(0, 0, -10);

        // Make the buttons
        buttons = GameObject.FindGameObjectsWithTag("Password");
        float angleAdjuster = Random.Range(-0.08f, 0.08f);
        float worldEdge = this.transform.localScale.x * -7.4f; // fixed amount of where the world edge is
        int num = 0;
        buttText = new TextMeshPro[buttons.Length];
        buttonPressed = new bool[buttons.Length];
        foreach (var button in buttons)
        {
            float angle = angleAdjuster * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, -1);
            button.transform.position = pos;
            button.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 180);

            // REPLACE THIS WHEN DONE DEBUG
            buttText[num] = button.GetComponentInChildren<TMPro.TextMeshPro>();
            //buttText[num].text = passwordButtLib[Random.Range(0, passwordButtLib.Length)];

            buttText[num].text = passwordButtLib[num];
            buttonPressed[num] = false;
            angleAdjuster -= 0.02f;
            num++;
        }
    }

    new void Update()
    {
        if (levelComplete)
            FadeOut(); // When level is complete we fade out and remove the level

        if (!puzzleSolved)
        {
            int i = 0;
            int totalCorrectButts = 0;

            Collider2D monsterCollider = Monster.instance.GetComponent<Collider2D>();
            foreach (var button in buttons)
            {
                ButtonPressCheck(button, i, monsterCollider); // Check and change when button pressed
                totalCorrectButts += ButtonCorrectChar(button, i); // Count all correct buttons chars
                i++;
            }

            puzzleSolved = SolveCheck(totalCorrectButts, password.Length);
        }
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
                {
                    buttText[index].text = passwordButtLib[currentCharIndex + 1];
                }
                else
                {
                    buttText[index].text = passwordButtLib[0];
                }

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

    bool SolveCheck(int keys, int locks)
    {
        if (Input.GetKey("s") || keys >= locks) // or if all buttons are correct
            return true;
        else
            return false;
    }
}
