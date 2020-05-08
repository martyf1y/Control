using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public static Monster instance = null;
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

        animator = GetComponent<Animator>();
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

    public void AnimationIdle(){
        animator.SetTrigger("EndPush");
    }

    // Update is called once per frame
    void Update(){
        // raycast 2d
        // DoWeMove();

    }
}
