using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Script : World
{
    public static Level2Script instance = null;

    // Generate 6 letterboxes
    private GameObject[] mailBoxes = new GameObject[6];
    public GameObject mailBoxPrefab;
    public Sprite mailOpenSprite;
    private GameObject[] newspapers;
    public GameObject newspaperPrefab;
    private float mailBoxWorldEdge;
    private float newspaperWorldEdge;
    private bool[] mailDelivered; // This tells us if the button has already been pressed.

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
        List<Color> mailColours = new List<Color>
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            new Color(1, .75f, 0.79f, 1f), // Pink
            new Color(1, .64f, 0f, 1f) // Orange 
        };

        List<Color> paperColour = new List<Color>(mailColours);
        mailBoxWorldEdge = this.transform.localScale.x * -9f; // fixed amount of where the world edge is
        newspaperWorldEdge = this.transform.localScale.x * -8f; // fixed amount of where the world edge is
        float start = 0.42f; // Left side of house
        float increment = .9f / mailBoxes.Length; // 90% of world is open
        for (int i = 0; i < mailBoxes.Length; i++)
        {
            mailBoxes[i] = MailMaker(mailBoxPrefab, mailColours, start, increment, mailBoxWorldEdge);
            newspapers[i] = MailMaker(newspaperPrefab, paperColour, start, increment, newspaperWorldEdge);

            start += increment;
            mailDelivered[i] = false;
        }
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyUp("up"))
        {
            float worldEdge = this.transform.localScale.x * -9f; // fixed amount of where the world edge is
            float start = 0.42f; // Right side of house
            float increment = .9f / mailBoxes.Length; // 90% of world is open
            float end = start + increment; // Start at halfway
            for (int i = 0; i < mailBoxes.Length; i++)
            {
                float rand; // placement of mailbox going for .42 - .32 (avoid house)
                rand = Random.Range(start, end) % 1; // ranges from 0.42 - 0.32

                start += increment;
                end += increment;

                float angle = rand * Mathf.PI * 2;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * worldEdge, Mathf.Sin(angle) * worldEdge, 0);
                mailBoxes[i].transform.position = pos;
                mailBoxes[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI + 270);

            }
        }
    }

    public void UpdatePickedUpPaper(Collider2D monCollider, Color colorOfPaperWithDog)
    {
        foreach (GameObject newspaper in newspapers)
        {
            Color newspaperCol = newspaper.GetComponent<SpriteRenderer>().color;
            if (colorOfPaperWithDog == newspaperCol)
            {
                float dogHeadAdjustment = 0.75f - (monCollider.transform.localScale.x * 0.015f); // We add in dog info to get newspaper to drop by dog head
                float angle = dogHeadAdjustment * Mathf.PI * 2;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * newspaperWorldEdge, Mathf.Sin(angle) * newspaperWorldEdge, 0);
                newspaper.transform.position = pos;
                newspaper.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI) + 270);
            }
        }
    }


    public Color PickupPaper(Collider2D monCollider, Color colorOfDog)
    {
        foreach (GameObject newspaper in newspapers)
        {
            // Optional - check dog head is on right side of newspaper (if dog facing right and newspaper x is less than dog
            if (monCollider.IsTouching(newspaper.GetComponent<Collider2D>())
                && DogHeadAbovePaper(monCollider, newspaper.GetComponent<Collider2D>()))
            {
                newspaper.GetComponent<SpriteRenderer>().enabled = false;
                return newspaper.GetComponent<SpriteRenderer>().color;
            }
        }
        return colorOfDog; // The dog 
    }

    public Color PaperDropped(Collider2D monCollider, Color colorOfPaperWithDog)
    {
        // int i = 0;
        foreach (GameObject newspaper in newspapers)
        {
            Color newspaperCol = newspaper.GetComponent<SpriteRenderer>().color;
            if (colorOfPaperWithDog == newspaperCol) // Drop only the one we have in mouth
            {
                if (CheckMailsOverMailBox(newspaper.GetComponent<Collider2D>(), newspaperCol))
                {
                    newspaper.SetActive(false);
                    return newspaperCol; // To send back to house to turn on light.
                }
                else newspaper.GetComponent<SpriteRenderer>().enabled = true; // Drop mail if its not in range of box
            }
        }
        return Color.white;
    }

    private bool CheckMailsOverMailBox(Collider2D npaperCol, Color nCol)
    {
        foreach (GameObject mailbox in mailBoxes)
        {
            if (mailbox.GetComponent<Collider2D>().IsTouching(npaperCol))
            {
                Debug.Log("true" + mailbox.GetComponent<SpriteRenderer>().color);
            }
            if (mailbox.GetComponent<Collider2D>().IsTouching(npaperCol) &&
                nCol == mailbox.GetComponent<SpriteRenderer>().color)
            {
                mailbox.GetComponent<SpriteRenderer>().sprite = mailOpenSprite;
                mailbox.GetComponent<SpriteRenderer>().color = Color.white;
                return true;
            }
        }
        return false;
    }

    public override bool PuzzleSolvedChecker()
    {
        // check all lights are on
        return false;
    }




    private bool DogHeadAbovePaper(Collider2D monColl, Collider2D newColl) =>
        (monColl.transform.localScale.x == -1 && newColl.transform.position.x < monColl.transform.position.x) ||
        (monColl.transform.localScale.x == 1 && newColl.transform.position.x > monColl.transform.position.x) ? true : false;

    GameObject MailMaker(GameObject prefab, List<Color> tempColors, float tempStart, float tempIncrement, float tempEdge)
    {
        GameObject newMailThing = Instantiate(prefab, transform) as GameObject;
        int ranCol = Random.Range(0, tempColors.Count);
        newMailThing.GetComponent<SpriteRenderer>().color = tempColors[ranCol];
        tempColors.RemoveAt(ranCol);

        float ranPlace = Random.Range(tempStart, tempStart + tempIncrement) % 1; // placement of mailbox going for .42 - .32 (avoid house)
        float angle = ranPlace * Mathf.PI * 2;
        Vector3 pos = new Vector3(Mathf.Cos(angle) * tempEdge, Mathf.Sin(angle) * tempEdge, 0);
        newMailThing.transform.position = pos;
        newMailThing.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(pos.y, pos.x) * 180 / Mathf.PI) + 270);
        return newMailThing;
    }
}
