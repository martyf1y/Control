using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public static Player instance = null; //Static instance allows it to be accessed by any other script.
    private Animator animator;
    private Rigidbody2D rg2d;

    // Movement variables
    private const float moveSpeed = .1f;
    private Vector3 mousePosition;
    bool playInteract = false;

    char facingThisWay = 'L';

    public delegate void Movement(); // What we swap between types of movement
    public Movement movement;

    // Level 2 variables
    bool moveBackDown = false;
    Vector3 moveHere = new Vector3(0, 16, 0);


    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        rg2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movement = GeneralMovement;
        rg2d.gravityScale = 0;
    }

    void Update()
    {
        if (gameObject.GetComponent<PolygonCollider2D>() == null) // Bad fix to polygon collider reset not doable
            gameObject.AddComponent<PolygonCollider2D>();

        movement();
    }

    void GeneralMovement()
    {
        if (Input.GetMouseButtonUp(1)) playInteract = !playInteract;

        if (playInteract)
        {
            rg2d.gravityScale = 0;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            mousePosition = GetWorldPositionOnPlane(Input.mousePosition, 0); // This fixes persepctive issues with Z axis
            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed); // Give a nice rubber band movement to mouse position                                                                              //   }
        }
        else rg2d.gravityScale = 0.1f;
    }

    public void Lvl1ToLvl2Transition()
    {
        if (Vector3.Magnitude(transform.position - moveHere) < 0.5) // when we are close enough
        {
            if (!moveBackDown)
            {  // spriteChange here
                AnimateHandEvolve();
                moveBackDown = true;

                moveHere = new Vector3(moveHere.x, moveHere.y - 6, moveHere.z);
                this.transform.rotation = Quaternion.Euler(0, 0, -90);
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                Destroy(gameObject.GetComponent<PolygonCollider2D>());
                movement = GeneralMovement; // Go back to normal movement
            }
        }
        else transform.position = Vector2.Lerp(transform.position, moveHere, 0.03f);
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        xy.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    public void Flip(char nowFaceThisWay)
    {
        if (facingThisWay != nowFaceThisWay)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingThisWay = nowFaceThisWay;
        }
    }

    public bool GetPlayInteract() => playInteract;

    public void StopInteract() => playInteract = false;

    public void AnimatePush() => animator.SetTrigger("StartPush");

    public void AnimateStopPush() => animator.SetTrigger("EndPush");

    void AnimateHandEvolve() => animator.SetTrigger("Evolve");
}
