using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = .1f;
    private Vector3 mousePosition;

    Rigidbody2D rg2d;
    bool playInteract = false;

    public static Player instance = null; //Static instance of allows it to be accessed by any other script.

    // Start is called before the first frame update
    void Start()
    {
         //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            Destroy(gameObject);

        rg2d = GetComponent<Rigidbody2D>();
    }

    public void PlayerInteraction()
    {
       
        if (playInteract)
        {
            // To add - If mouse is over world then do not do this 
            rg2d.gravityScale = 0;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
        }
        else
        {
            rg2d.gravityScale = 0.1f;
        }
    }

    public bool GetPlayInteract()
    {
        return playInteract;
    }

    // Update is called once per frame
    void Update()
    {

        // If player is coming into contact with the monster
        // World objects all rotate
        if (Input.GetMouseButtonUp(1))
        {
            playInteract = !playInteract;
        }
        PlayerInteraction();
    }

}
