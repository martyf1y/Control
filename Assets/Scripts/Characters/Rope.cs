using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.1f;
    public const int maxLength = 20;
    private int segmentLength = maxLength;
    private float lineWidth = 0.06f;
    private float gravity = -8f;
    public Transform startPoint;
    public Transform endPoint;
    private int expectedRopeSize;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();

        startPoint = this.transform.parent;
        
        endPoint = GameObject.Find("Monster/Collar").transform;
        Debug.Log(startPoint);
        Debug.Log(endPoint);

        //  Vector3 ropeStartPoint = startPoint.transform.position;
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
        expectedRopeSize = segmentLength;
    }

    // Update is called once per frame
    void Update()
    {
        CheckRopeAdjust(expectedRopeSize);
        this.Simulate();
        this.DrawRope();
    }

    public void CheckRopeAdjust(int amount)
    {
        if (segmentLength == amount) return; 

        segmentLength += (segmentLength > amount ? -1 : 1);
        expectedRopeSize = amount;
    }

    private void FixedUpdate()
    {
       
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 forceGravity = new Vector2(0f, gravity);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }


    private void ApplyConstraint()
    { 
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = this.startPoint.position;
        this.ropeSegments[0] = firstSegment;

        RopeSegment endSegment = this.ropeSegments[this.segmentLength - 1];
        endSegment.posNow = this.endPoint.position;
        this.ropeSegments[this.segmentLength - 1] = endSegment;


        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = dist - ropeSegLen;
            Vector2 changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;


            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
