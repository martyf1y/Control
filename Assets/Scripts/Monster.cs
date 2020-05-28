using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public static Monster instance = null;
    private Animator animator;
    char facingThisWay = 'R';


    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // raycast 2d
        // DoWeMove();

        if (gameObject.GetComponent<PolygonCollider2D>() == null) // Bad fix to polygon collider reset not doable
        {
            gameObject.AddComponent<PolygonCollider2D>();
        }
    }

    public void Animate(char goThisWay){
        if (goThisWay == 'L')
        {
            animator.SetTrigger("PushLeft");
        }
        else if (goThisWay == 'R')
        {
            animator.SetTrigger("PushRight");
        }
    }

    public void Flip(char nowFaceThisWay)
    {
        if (facingThisWay != nowFaceThisWay)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingThisWay = nowFaceThisWay;
        }
    }

    public void AnimationIdle(){
        animator.SetTrigger("EndPush");
    }

    public void AnimateEvolve()
    {
        animator.SetTrigger("Evolve");
    }

   

    void GoBig() // Used in end of animation "Monster Evolve" in Animation controller
    {
        this.transform.localScale = new Vector3(1.8f, 1.8f, 1);
        Destroy(gameObject.GetComponent<PolygonCollider2D>()); // MIGHT BE EASIER TO JUST LOAD A PREFAB THAT HAS ALL THE RIGHT STUFF
        Destroy(gameObject.GetComponentInChildren<CircleCollider2D>());
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // No more moving the monster round
    }
}
