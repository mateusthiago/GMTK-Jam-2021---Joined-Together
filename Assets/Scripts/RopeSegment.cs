using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    public int index;
    public CircleCollider2D reconnectTrigger;    
    public HingeJoint2D joint;      
    public float maxDistance;
    public float distance;
    public bool unbreakable;
    public bool isBroken = false;
    public bool isRemoved;
    public List<Collider2D> overlaps;

    public RopeSegment previousSegment;
    public RopeSegment nextSegment;
    Rope rope;

    void Start()
    {
        rope = transform.parent.GetComponent<Rope>();        
        joint = GetComponent<HingeJoint2D>();
        reconnectTrigger = GetComponent<CircleCollider2D>();
        reconnectTrigger.enabled = false;
    }

    
    void Update()
    {
        if (unbreakable) return;

        if (rope.isBroken == false)
        {
            float currentForce = joint.reactionForce.magnitude;
            if (currentForce > rope.breakForce)
            {
                print(this.name + " broke with " + currentForce + " force");
                rope.Break(this);
            }
        }
    }
    

    public void TryReconnect(float force)
    {
        Vector2 distance = previousSegment.transform.position - transform.position;
        if (distance.sqrMagnitude < rope.reconnectDistance)
        {
            joint.enabled = true;
            isBroken = false;
            rope.CheckIntegrity();
        }

        Vector2 myDirection = distance.normalized;
        joint.attachedRigidbody.AddForce(myDirection * force, ForceMode2D.Impulse);

        Vector2 previousSegmentDirection = myDirection * -1;
        previousSegment.joint.attachedRigidbody.AddForce(previousSegmentDirection * force, ForceMode2D.Impulse);
    }

    public static int SortByIndex(RopeSegment a, RopeSegment b)
    {
        return a.index.CompareTo(b.index);
    }
}
