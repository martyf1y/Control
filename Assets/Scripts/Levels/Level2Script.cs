using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Script : World
{
    public static Level2Script instance = null;

    // Generate 6 letterboxes
    private GameObject[] mailBoxes = new GameObject[6];
    public GameObject mailBoxPrefab;
    private GameObject[] newspapers;
    public GameObject newspaperPrefab;
    private bool[] mailDelivered; // This tells us if the button has already been pressed.
    private bool[] thisNewspaperInMouth;
    private bool newspaperInMouth = false;
    


    void Start()
    {
        //Check if instance already exists
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        MonsterView = new Vector3(0, 10f, -130); // Change parent variables to the level settings.
        WorldView = new Vector3(0, 0, -590);
        Rotation = new Vector3(0, 0, 8);
        this.transform.eulerAngles = new Vector3(0, 0, 220);

        // ---------------- Create the Mail ---------------- //
        newspapers = new GameObject[mailBoxes.Length];
        mailDelivered = new bool[mailBoxes.Length];
        thisNewspaperInMouth = new bool[mailBoxes.Length];
        List<Color> mailColours = new List<Color>();
        mailColours.Add(Color.red);
        mailColours.Add(Color.green);
        mailColours.Add(Color.blue);
        mailColours.Add(Color.yellow);
        mailColours.Add(new Color(1, .75f, 0.79f, 1f)); // Pink
        mailColours.Add(new Color(1, .64f, 0f, 1f)); // Orange 

        List<Color> newspaperColour = new List<Color>(mailColours);
        float mailBoxWorldEdge = this.transform.localScale.x * -9f; // fixed amount of where the world edge is
        float newspaperWorldEdge = this.transform.localScale.x * -7.8f; // fixed amount of where the world edge is
        float start = 0.42f; // Left side of house
        float increment = .9f / mailBoxes.Length; // 90% of world is open
            for (int i = 0; i < mailBoxes.Length; i++)
            {  
                mailBoxes[i] = MailMaker(mailBoxPrefab, mailColours, start, increment, mailBoxWorldEdge);
                newspapers[i] = MailMaker(newspaperPrefab, newspaperColour, start, increment, newspaperWorldEdge);

                start += increment;
                mailDelivered[i] = false;
                thisNewspaperInMouth[i] = false;
            }
        }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyUp("up")){
            float worldEdge = this.transform.localScale.x * -9f; // fixed amount of where the world edge is
            float start = 0.42f; // Right side of house
            float increment = .9f / mailBoxes.Length; // 90% of world is open
            float end = start + increment; // Start at halfway
            for (int i = 0; i < mailBoxes.Length; i++)
            {
              //  Debug.Log("One " + start%1);

                float rand; // placement of mailbox going for .42 - .32 (avoid house)
                rand = Random.Range(start, end)%1; // ranges from 0.42 - 0.32

                start += increment;
                end += increment;

                float angle = rand * Mathf.PI * 2;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, -1);
                mailBoxes[i].transform.position = pos;
                mailBoxes[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 270);

            }
        }
    }


    public override void PuzzleInteraction(Collider2D monCollider, Collider2D playerCollider)
    {
        if (HandHitDogFromAbove(monCollider, playerCollider) ) // Optional - add speed of hit check
        {
            int i = 0;
            foreach (GameObject newspaper in newspapers)
            {
                if (!newspaperInMouth)
                {
                    // Optional - check dog head is on right side of newspaper (if dog facing right and newspaper x is less than dog
                    if (monCollider.IsTouching(newspaper.GetComponent<Collider2D>()))
                    {
                        // Change Dog sprite 
                        // Change dog colour
                        // Disappear newspaper
                        newspaperInMouth = true;
                        thisNewspaperInMouth[i] = true;
                    }
                }
                else if(thisNewspaperInMouth[i])
                {
                    // Place it down
                    // Change Dig Sprite
                    thisNewspaperInMouth[i] = false;
                }
                i++;
            }
            
        }
        foreach (GameObject newspaper in newspapers)
        {

        }

        }

    public override bool PuzzleSolvedChecker()
    {
       
        return false;
    }

    private bool HandHitDogFromAbove(Collider2D cColl, Collider2D pColl) =>
    pColl.IsTouching(cColl) && (pColl.transform.position.y > cColl.transform.position.y) ? true : false; // Is player hitting dog from above?

    

        GameObject MailMaker(GameObject prefab, List<Color> tempColors, float tempStart, float tempIncrement, float tempEdge)
    {
        GameObject newMailThing = Instantiate(prefab, transform) as GameObject;
        int ranCol = Random.Range(0, tempColors.Count);
        newMailThing.GetComponent<SpriteRenderer>().color = tempColors[ranCol];
        tempColors.RemoveAt(ranCol);

        float ranPlace = Random.Range(tempStart, (tempStart + tempIncrement)) % 1; // placement of mailbox going for .42 - .32 (avoid house)
        float angle = ranPlace * Mathf.PI * 2;
        Vector3 pos = new Vector3(Mathf.Cos(angle) * tempEdge, Mathf.Sin(angle) * tempEdge, -1);
        newMailThing.transform.position = pos;
        newMailThing.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI) + 270);
        return newMailThing;
    }
}
