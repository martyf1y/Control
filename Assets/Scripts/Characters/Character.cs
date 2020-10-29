using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected char facingThisWay = 'L';
    protected Animator animator;
    public delegate void Move(char direction);
    public Move move;
    void Start() { }

    public virtual void Update()
    {
        if (gameObject.GetComponent<PolygonCollider2D>() == null) // Bad fix to polygon collider reset not doable
            gameObject.AddComponent<PolygonCollider2D>();
    }

    public void UpdateMove(int level)
    {
        if (level == 1) move = L1Move;
        else if (level == 2) move = L2Move;
        else if (level == 3) { }
        else move = L1Move;
    }

    public virtual void L1Move(char direction) { }
    public virtual void L2Move(char direction) { }

    public void AnimateEvolve() => animator.SetTrigger("Evolve");

    public void Flip(char nowFaceThisWay)
    {
        if (facingThisWay != nowFaceThisWay)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingThisWay = nowFaceThisWay;
        }
    }

}
