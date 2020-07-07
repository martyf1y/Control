using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public static Monster instance = null;
    private Animator animator;
    char facingThisWay = 'R';
    public bool scaleMonster = false;

    void Start()
    {
        //Check if instance already exists
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (gameObject.GetComponent<PolygonCollider2D>() == null) // Bad fix to polygon collider reset not doable
            gameObject.AddComponent<PolygonCollider2D>();

        if (scaleMonster) // One off fix for sprite size difference
            scaleMonster = ScaleMonster(new Vector3(0.21f, 0.21f, 1));
    }

    public void Animate(char goThisWay) => animator.SetBool(goThisWay == 'L' ? "PushLeftB" : "PushRightB", true);

    public void Flip(char nowFaceThisWay)
    {
        if (facingThisWay != nowFaceThisWay)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingThisWay = nowFaceThisWay;
        }
    }

    public void AnimationIdle()
    {
        animator.speed = 1;
        animator.SetBool("PushRightB", false);
        animator.SetBool("PushLeftB", false);
    }

    public void AnimateEvolve()
    {
        gameObject.GetComponentInChildren<CircleCollider2D>().offset = new Vector2(0, -.32f);
        animator.speed = 5;
        animator.SetTrigger("Evolve");
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
}
