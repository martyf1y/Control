using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public static Monster instance = null;
    private Animator animator;
    public bool makeMonsterBigger = false;
    private float thisBig;
    bool once = false;


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

    public void AnimateEvolve()
    {
        animator.SetTrigger("Evolve");
    }

    void MakeMonsterBigger(float howBig)
    {
        makeMonsterBigger = true;
        thisBig = howBig;
    }


    // Update is called once per frame
    void Update(){
        // raycast 2d
        // DoWeMove();

        if (!gameObject.GetComponent<PolygonCollider2D>()) // Stupid fix to sprite sizing issue
        {
            gameObject.AddComponent<PolygonCollider2D>();
            
            once = true;
        }
    }

    void GoBig() // Used in end of animation "Monster Evolve" in Animation controller
    {
        Destroy(GetComponent<PolygonCollider2D>()); // MIGHT BE EASIER TO JUST LOAD A PREFAB THAT HAS ALL THE RIGHT STUFF
        Destroy(GetComponentInChildren<CircleCollider2D>());
        this.transform.localScale = new Vector3(1.8f, 1.8f, 1);
    }
}
