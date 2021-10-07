using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    public Rigidbody2D p1, p2;
    public int ropePoints;
    public float pointDistance;
    public float width;
    public int constraintIterations;
    public float gravity;

    private RopePoint[] pointArray;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        pointArray = new RopePoint[ropePoints];
        line.positionCount = pointArray.Length;
        Vector2 position = transform.position;
        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = new RopePoint(position);            
            position.y -= pointDistance;
        }        
             
    }

    private void FixedUpdate()
    {
        SimulateRope();
        for (int i = 0; i < constraintIterations; i++)
        {
            ApplyConstraints();
            //CheckMaxLength();
        }
        DrawRope();
    }

    void DrawRope()
    {
        line.startWidth = line.endWidth = width;

        for (int i = 0; i < pointArray.Length; i++)
        {
            line.SetPosition(i, pointArray[i].currentPosition);
        }
    }

    void SimulateRope()
    {
        for (int i = 0; i < pointArray.Length; i++)
        {
            RopePoint point = pointArray[i];
            Vector2 oldPosition = point.currentPosition;
            Vector2 velocity = point.currentPosition - point.oldPosition;
            point.currentPosition += velocity + Vector2.down * gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            point.oldPosition = oldPosition;
        }
    }

    void CheckMaxLength()
    {
        RopePoint firstPoint = pointArray[0];
        RopePoint lastPoint = pointArray[pointArray.Length - 1];
        float currentDistance = Vector2.Distance(firstPoint.currentPosition, lastPoint.currentPosition);        
        float maxLength = ropePoints * pointDistance;        
        if (currentDistance > maxLength)
        {
            float difference = (currentDistance - maxLength)/currentDistance;
            Vector2 direction = (p1.position - p2.position).normalized;
            p1.AddForce(-(difference * direction) * 0.5f, ForceMode2D.Impulse);
            p2.AddForce((difference * direction) * 0.5f, ForceMode2D.Impulse);
        }        
        firstPoint.currentPosition = p1.position;
        lastPoint.currentPosition = p2.position;
    }
    
    void ApplyConstraints()
    {
        pointArray[0].currentPosition = p1.position;
        pointArray[pointArray.Length - 1].currentPosition = p2.position;

        for (int i = 0; i < pointArray.Length - 1; i++)
        {
            RopePoint point1 = pointArray[i];
            RopePoint point2 = pointArray[i + 1];
            float distanceX = point1.currentPosition.x - point2.currentPosition.x;
            float distanceY = point1.currentPosition.y - point2.currentPosition.y;
            float distance = Vector2.Distance(point1.currentPosition, point2.currentPosition);
            float difference = 0;
            if (distance > 0)
            {
                difference = (pointDistance - distance) / distance;
            }

            Vector2 correction = new Vector2(distanceX, distanceY) * (0.5f * difference);

            point1.currentPosition += correction;
            point2.currentPosition -= correction;
        }
    }
}

public class RopePoint
{
    public Vector2 oldPosition, currentPosition;

    public RopePoint(Vector2 position)
    {
        this.currentPosition = this.oldPosition = position;
    }
}
