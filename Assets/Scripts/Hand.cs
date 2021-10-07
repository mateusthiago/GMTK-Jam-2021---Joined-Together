using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Body body;
    public Rigidbody2D mainRigidbody;
    public Transform holdPoint;
    public HingeJoint2D hookJoint;
    public int hookLayer;
    public int deathLayer = 20;
    public int winLayer = 21;
    public float distance;
    private Rigidbody2D myRigidbody;
    private HingeJoint2D currentHook;
    private Rope rope;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        body.ReleaseEvent += Release;
        rope = FindObjectOfType<Rope>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (body.tryHolding && !body.isHolding)
        {
            if (collider.gameObject.layer == hookLayer)
            {
                distance = (collider.transform.position - transform.position).magnitude;
                Vector2 direction = (collider.transform.position - transform.position).normalized;
                myRigidbody.AddForce(direction * 1000);

                if (distance < 0.4f)
                {
                    body.isHolding = true;
                    currentHook = collider.gameObject.GetComponent<HingeJoint2D>(); ;
                }
                else
                {
                    body.isHolding = false;
                    currentHook = null;
                }
            }
        }
        else if (body.isHolding && currentHook != null)
        {
            //Vector2 direction = (currentHook.position - transform.position).normalized;
            //myRigidbody.AddForce(direction * 2000);            
            currentHook.connectedBody = myRigidbody;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == deathLayer)
        {
            GameManager.instance.Death();
        }
        if (collision.gameObject.layer == winLayer && rope.isBroken == false)
        {
            GameManager.instance.Win();
        }
    }

    private void Release()
    {
        if (currentHook != null) currentHook.connectedBody = null;
        body.isHolding = false;
        currentHook = null;

        if (hookJoint != null)
        {                
            hookJoint.connectedBody = null;
            hookJoint = null;
            body.isHolding = false;
        }
    }
}
