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
        else if (instance != this) Destroy(this);

        facingThisWay = 'R';
        animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();
        // One off fix for sprite size difference
        if (scaleMonster) scaleMonster = ScaleMonster(new Vector3(0.22f, 0.22f, 1));
    }
    public bool HitOnHead(Collider2D obj, float oSpeed)
    {
        Vector2 oPos = obj.transform.position;
        Vector2 pos = GetComponent<Collider2D>().transform.position;
        float dir = GetComponent<Collider2D>().transform.localScale.x;
        bool headSide = (dir == -1 && oPos.x < pos.x) || (dir == 1 && oPos.x > pos.x);
        bool onHead = oPos.y > pos.y && headSide;
        return obj.IsTouching(GetComponent<Collider2D>()) && onHead && oSpeed > 1f;
    }


    // ------------------------------ Animations  ---------------------------------- //
    public override void L1Move(char direction) => animator.SetBool(direction == 'L' ? "PushLeftB" : "PushRightB", true);
    public override void L2Move(char direction)
    {
        Flip(direction);
        animator.SetBool(direction == 'L' ? "PushLeftB" : "PushRightB", true);
    }
    public void StopPush()
    {
        animator.speed = 1;
        animator.SetBool("PushRightB", false);
        animator.SetBool("PushLeftB", false);
    }

    public void GrabNewspaper() => animator.SetBool("WithPaperB", true);
    public void DropNewspaper() => animator.SetBool("WithPaperB", false);

    // ------------------------------ Animations  ---------------------------------- //

    // ------------------------------ Level 1 Evolve Function ---------------------------------- //
    public void Evolve(int levelNum)
    {
        if (levelNum == 1)
        {
            scaleMonster = true; // Bad fix to scale issue between sprites
            this.GetComponent<CircleCollider2D>().offset = new Vector2(0, -2f); // Has dog bounce higher up
            this.GetComponent<CircleCollider2D>().sharedMaterial = null; // Has dog bounce higher up
            animator.speed = 5;
            AnimateEvolve();
        }
    }

    bool ScaleMonster(Vector3 targetScale)
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, 0.09f);
        return this.transform.localScale == targetScale ? false : true;
    }

    void GoBig() // Used in end of animation "Monster Evolve" in Animation controller
    {
        this.transform.localScale = new Vector3(1f, 1f, 1);
        Destroy(this.GetComponent<PolygonCollider2D>());
        Destroy(this.GetComponent<CircleCollider2D>());
        scaleMonster = false; // If ScaleMonster does not finish in time
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // No more moving the monster round
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        transform.position = new Vector3(0, 8.48f, 0);
        transform.GetChild(0).gameObject.SetActive(true); // Collar
        transform.GetChild(0).gameObject.transform.position = new Vector3((transform.position.x + .54f), (transform.position.y + .24f), 0f);
    }
    // ------------------------------ Level 1 Evolve Function ---------------------------------- //

}
