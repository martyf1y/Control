using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public static Monster instance = null;
    private Collider2D monsterCollider;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this; //if not, set instance to this
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance.
            Destroy(gameObject);

        monsterCollider = this.GetComponent<Collider2D>();

        animator = GetComponent<Animator>();
    }

    
    public char DoWeMove(Collider2D worldCol, Collider2D playerCol, bool playInteract)
    {
        char whichWay = 'N';
        //Debug.Log("collider " + monsterCollider.IsTouching(playerCollider));
        if (monsterCollider.IsTouching(worldCol) // Are we on the floor?
            && monsterCollider.IsTouching(playerCol)
            && playInteract) // Is the player pushing us?
      
        {
            float playerPos = playerCol.transform.position.x;
            float monsterPos = this.transform.position.x;
            if (monsterPos < playerPos)
            {
                AnimateLeft();
                whichWay = 'L';
            }
            else
            {
                AnimateRight();
                whichWay = 'R';
            }
        }
        else
        {
            AnimationIdle();
        }
        return whichWay;
    }

    void AnimateLeft()
    {
        animator.SetTrigger("PushLeft");
    }

    void AnimateRight()
    {
        animator.SetTrigger("PushRight");

    }
    void AnimationIdle()
    {
        animator.SetTrigger("EndPush");
    }

    // Update is called once per frame
    void Update()
    {
        //         // raycast 2d
       // DoWeMove();

    }
}
