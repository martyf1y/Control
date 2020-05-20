using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public static Player instance = null; //Static instance allows it to be accessed by any other script.
    private Animator animator;
    private Rigidbody2D rg2d;


    private const float moveSpeed = .1f;
    private Vector3 mousePosition;

    bool playInteract = false;
    char facingThisWay = 'L';


    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            Destroy(gameObject);

        rg2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void PlayerMovement()
    {
        if (playInteract)
        {
            rg2d.gravityScale = 0;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = GetWorldPositionOnPlane(Input.mousePosition, 0); // This fixes persepctive issues with Z axis

            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed); // Give a nice rubber band movement to mouse position
        }
        else
            rg2d.gravityScale = 0.1f;
    }

    public bool GetPlayInteract()
    {
        return playInteract;
    }

    public void StopInteract()
    {
        playInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If player is coming into contact with the monster
        // World objects all rotate
        if (Input.GetMouseButtonUp(1))
            playInteract = !playInteract;

        PlayerMovement();
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
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

    public void AnimatePush()
    {
        animator.SetTrigger("StartPush");
    }

    public void AnimateStopPush()
    {
        animator.SetTrigger("EndPush");
    }

}
