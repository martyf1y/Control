using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularGravity : MonoBehaviour
{
    private Transform target; // Big object
    Vector3 targetDirection;

    private int forceAmount = 1500;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        target = GameObject.Find("CenterOfGravity").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDirection = target.position - transform.position; // Save direction
        targetDirection = targetDirection.normalized; // Normalize target direction vector
        rb.AddForce(targetDirection * forceAmount * Time.deltaTime);
    }
}
