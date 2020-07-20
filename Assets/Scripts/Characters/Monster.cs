using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    public static Monster instance = null;

    [HideInInspector] public bool scaleMonster = false;

    void Start()
    {
        //Check if instance already exists
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        facingThisWay = 'R';

        animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

        if (scaleMonster) // One off fix for sprite size difference
            scaleMonster = ScaleMonster(new Vector3(0.21f, 0.21f, 1));
    }

    public void AnimatePush(char goThisWay) => animator.SetBool(goThisWay == 'L' ? "PushLeftB" : "PushRightB", true);

    public void AnimateStopPush()
    {
        animator.speed = 1;
        animator.SetBool("PushRightB", false);
        animator.SetBool("PushLeftB", false);
    }


    // ------------------------------ Level 1 Evolve Function ---------------------------------- //

    public void Evolve(int levelNum)
    {
        if (levelNum == 1)
        {
            scaleMonster = true; // Bad fix to scale issue between sprites
            gameObject.GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0, -.32f);
            animator.speed = 5;
            AnimateEvolve();
        }
    }

    bool ScaleMonster(Vector3 targetScale)
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, 0.09f);
        return (this.transform.localScale == targetScale ? false : true);
    }

    void GoBig() // Used in end of animation "Monster Evolve" in Animation controller
    {
        this.transform.localScale = new Vector3(1f, 1f, 1);
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        Destroy(gameObject.GetComponentInChildren<CircleCollider2D>());
        scaleMonster = false; // If ScaleMonster does not finish in time
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // No more moving the monster round
    }
    // ------------------------------ Level 1 Evolve Function ---------------------------------- //

}
