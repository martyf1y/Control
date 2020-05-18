using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1Script : World
{
    public static Level1Script instance = null;
    /// <summary>
    /// Create new gameobjects based on the to be made button class
    /// Have a button check that takes monster collider
    /// Trigger a change eveythime collider hits
    /// Run cheker in manager
    /// // Change the gameobject to the new one and call the functions when needed in the sections that are relevant for it.

    /// world1.update
    /// </summary>

    //public GameObject prefabButton;
    private GameObject[] buttons;
    private float posBasedOnWorld;
    private float speed = 0.1f;

    // Button related
    TextMeshPro[] buttText;
    private string[] passwordButtLib = {"P", "A", "S", "W", "O", "R", "D"};
    string password = "PASSWORD"; // The Password to get right

    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);


        monsterView = new Vector3(0, 8.5f, -80); // Change parent variables to the level settings.
        worldView = new Vector3(0, 0, -450);
        rotation = new Vector3(0, 0, -10);

        buttons = GameObject.FindGameObjectsWithTag("Password");
        float angleAdjuster = Random.Range(-0.08f, 0.08f);

        float worldEdge = this.transform.localScale.x * -7.4f; // fixed amount of where the world edge is
        int num = 0;
        buttText = new TextMeshPro[buttons.Length];
        foreach (var button in buttons)
        {
            float angle = angleAdjuster * Mathf.PI * 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, -1);
            button.transform.position = pos;
            button.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 180);
            // Change the text randomly
            buttText[num] = button.GetComponentInChildren<TMPro.TextMeshPro>();
            buttText[num].text = passwordButtLib[Random.Range(0, passwordButtLib.Length)];
            angleAdjuster += 0.02f;
            num++;

        }
    }


 

    // Update is called once per frame
    new void Update()
    {

        if (!puzzleSolved)
        {
            int i = 0;
            int passCharCorrect = 0;

            Collider2D monsterCollider = Monster.instance.GetComponent<Collider2D>();
            foreach (var button in buttons)
            {
                Collider2D buttCollider = button.GetComponent<Collider2D>();
                if (monsterCollider.IsTouching(buttCollider))
                {
                    Debug.Log("Touching!! " + i + " " + button.transform.position);
                    // Change button only once
                    int currentCharIndex = System.Array.IndexOf(passwordButtLib, buttText[i].text);
                    if (currentCharIndex != passwordButtLib.Length-1)
                    {
                        buttText[i].text = passwordButtLib[currentCharIndex+1];
                    }
                    else
                    {
                        buttText[i].text = passwordButtLib[0];
                    }
                    
                    // Check if Button is correct type
                    // Have counter for all buttons that returns true
                    if (buttText[i].text[0] == password[i])
                    {
                        passCharCorrect += 1;
                        // colour is green
                    }
                    else
                    {
                        // colour is red
                    }
                }
                i++;
            }

            if (Input.GetKey("s") || passCharCorrect >= password.Length) // or if all buttons are correct
            {
                puzzleSolved = true;
            }
        }
         
        //PuzzleAction();
    }

    public void ChangeLetter(int num)
    {
        //  WBSpriteRenderer.sprite = WBSprite[num];
    }

    void PuzzleAction(GameObject other) // This will be where the program does the specific puzzle check
    {


    }

    void setupButton(GameObject thisbutton, int index, float thisRadius, float thisAngleAdj, TextMeshPro thisButtText)
    {
          
    }
}
