using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player instance = null; //Static instance allows it to be accessed by any other script.
    private Rigidbody2D rg2d;

    // Movement variables
    private const float moveSpeed = .1f;
    private Vector3 mousePosition;
    private Vector3 prevPos;
    public bool PlayInteract {get;set;}
    public bool AttemptedHit { get; set;}

    public delegate void Movement(); // What we swap between types of movement
    public Movement movement;

    // Level 2 variables
    bool moveBackDown = false;
    Vector3 moveHere = new Vector3(0, 16, 0);
    public float Force { get; set; }
    public Transform attachCollarHere;
    [SerializeField] private GameObject leash = null;

    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        rg2d = GetComponent<Rigidbody2D>();
        movement = GeneralMovement;
        rg2d.gravityScale = 0;
        animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        movement();
    }

    void GeneralMovement()
    {
        if (Input.GetMouseButtonUp(1)) PlayInteract = !PlayInteract; //rg2d.velocity = Vector3.zero; //rg2d.angularVelocity = 0;

        if (PlayInteract)
        {
            if (Force < 10)
            {
                this.GetComponent<CircularGravity>().enabled = false;
                rg2d.velocity = new Vector2(0, 0);
                rg2d.angularVelocity = 0;
                rg2d.gravityScale = 0;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                mousePosition = GetWorldPositionOnPlane(Input.mousePosition, 0); // This fixes persepctive issues with Z axis
                transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed); // Give a nice rubber band movement to mouse position  
                mousePosition = mousePosition.normalized;
                prevPos = transform.position;
            }
            else ApplyForce();

        }
        else this.GetComponent<CircularGravity>().enabled = true;
    }

    public void HitAttack()
    {
        if (Force<10)
        Force = 300;
    }

    private void ApplyForce() => rg2d.AddForce(new Vector2(0, -(Force *= .88f)));

public void Evolve()
    {
    // if (levelNum == 1) {
        rg2d.velocity = new Vector2(0, 0);
        this.GetComponent<CircularGravity>().enabled = false;
    if (Vector3.Magnitude(transform.position - moveHere) < 0.5) // when we are close enough
        {
            if (!moveBackDown)
            {  // spriteChange here
                AnimateEvolve();
                moveBackDown = true;

                this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // No more moving the monster round
               // moveHere = new Vector3(0.59f, 8.6f, 0);
                moveHere = attachCollarHere.position;
                this.transform.rotation = Quaternion.Euler(0, 0, 270);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    
            }
            else
            {
                Destroy(this.GetComponent<PolygonCollider2D>());
                this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                AddLeash();
                movement = GeneralMovement; // Go back to normal movement
                this.GetComponent<CircularGravity>().enabled = true;
                PlayInteract = true;
            }
        }
        else transform.position = Vector2.Lerp(transform.position, moveHere, 0.04f);
    //  }
    }

    void AddLeash() => leash = Instantiate(leash, transform) as GameObject;

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        xy.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    public void AnimatePush() => animator.SetBool("Push", true);  //animator.SetTrigger("EndPush");

    public void AnimateStopPush() => animator.SetBool("Push", false);  //animator.SetTrigger("EndPush");

}
