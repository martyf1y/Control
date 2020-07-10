using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    protected char facingThisWay = 'L';
    protected Animator animator;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (gameObject.GetComponent<PolygonCollider2D>() == null) // Bad fix to polygon collider reset not doable
            gameObject.AddComponent<PolygonCollider2D>();
    }


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
