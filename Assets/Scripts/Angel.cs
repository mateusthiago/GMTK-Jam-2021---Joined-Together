using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    public Transform endPositionTransform;
    public float moveSpeed;

    public Vector2 startPosition;
    public Vector2 endPosition;
    public float moveTimer;
    public bool invert;

    private void Start()
    {        
        startPosition = transform.position;
        endPosition = endPositionTransform.position;
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!invert) moveTimer += Time.deltaTime * moveSpeed;
        else moveTimer -= Time.deltaTime * moveSpeed;

        float moveX = Mathf.SmoothStep(startPosition.x, endPosition.x, moveTimer);
        float moveY = Mathf.SmoothStep(startPosition.y, endPosition.y, moveTimer);        
        
        myRigidbody.MovePosition(new Vector2(moveX, moveY));

        if (moveTimer > 1)
        {
            moveTimer = 1;
            invert = !invert;
        }

        if (moveTimer < 0)
        {
            moveTimer = 0;
            invert = !invert;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(startPosition, 0.25f);
            Gizmos.DrawSphere(endPosition, 0.25f);
            Gizmos.DrawLine(startPosition, endPosition);
        }
        else
        {            
            Gizmos.DrawSphere(transform.position, 0.25f);
            Gizmos.DrawSphere(endPositionTransform.position, 0.25f);
            Gizmos.DrawLine(transform.position, endPositionTransform.position);
        }
    }
}
