using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int maxLength;
    public int unbreakableLength;
    public float breakForce;
    public float reconnectForce;
    public float reconnectDistance;
    public float ropeControlInterval;
    public float intervalCountForRopeControl;
    public Rigidbody2D body1;
    public Rigidbody2D body2;
    public DistanceJoint2D distanceJoint;
    public ParticleSystem breakParticles;
    public List<RopeSegment> segmentsList = new List<RopeSegment>();    
    public RopeSegment brokenSegment;
    public bool isBroken = false;

    public List<RopeSegment> breakableSegments = new List<RopeSegment>();
    public List<RopeSegment> removedSegments = new List<RopeSegment>();

    private SoundManager soundManager;

    ContactFilter2D contactFilter;
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        DefineAndPositionSegments();

        LayerMask mask = LayerMask.GetMask("Rope");
        contactFilter.SetLayerMask(mask);
        contactFilter.useTriggers = true;
    }

    public void DefineAndPositionSegments()
    {       
        Vector2 direction = (body2.transform.position - body1.transform.position).normalized;

        for (int i = 0; i < maxLength; i++)
        {
            segmentsList.Add(transform.GetChild(i).GetComponent<RopeSegment>());
            segmentsList[i].index = i;

            if (i < unbreakableLength || i >= maxLength - unbreakableLength) segmentsList[i].unbreakable = true;
            else breakableSegments.Add(segmentsList[i]);

            if (i > 0)
            {
                segmentsList[i].previousSegment = segmentsList[i - 1];
                segmentsList[i - 1].nextSegment = segmentsList[i];
            }

            segmentsList[i].transform.position = (Vector2)body1.transform.position + segmentsList[0].joint.connectedAnchor + direction * 0.05f * i;
        }

        body2.transform.position = (Vector2)segmentsList[segmentsList.Count - 1].transform.position - body2.GetComponent<HingeJoint2D>().anchor;
    }

    public void PullTogether()
    {
        if (brokenSegment != null)
        {
            Lengthen();
            brokenSegment.TryReconnect(reconnectForce);
            return;
        }        
    }        

    public void SpontaneousBreak()
    {
        if (isBroken == false)
        {
            RopeSegment segmentToBreak = breakableSegments[breakableSegments.Count/2];
            Break(segmentToBreak);
            segmentToBreak.joint.attachedRigidbody.AddForce(Vector2.up * reconnectForce * 1.5f, ForceMode2D.Impulse);
            segmentToBreak.previousSegment.joint.attachedRigidbody.AddForce(Vector2.up * reconnectForce * 1.5f, ForceMode2D.Impulse);

            print("Rope broke at " + segmentToBreak.gameObject.name);
        }
    }

    bool alternateShortenSide;
    public void Shorten()
    {
        if (isBroken) return;
        if (breakableSegments.Count > 1)
        {
            intervalCountForRopeControl += Time.deltaTime;
            if (intervalCountForRopeControl >= ropeControlInterval)
            {
                RopeSegment segmentToRemove = alternateShortenSide ? breakableSegments[0] : breakableSegments[breakableSegments.Count - 1];

                segmentToRemove.previousSegment.nextSegment = segmentToRemove.nextSegment;
                segmentToRemove.nextSegment.previousSegment = segmentToRemove.previousSegment;
                segmentToRemove.nextSegment.joint.connectedBody = segmentToRemove.previousSegment.joint.attachedRigidbody;

                breakableSegments.Remove(segmentToRemove);
                removedSegments.Add(segmentToRemove);
                removedSegments.Sort(RopeSegment.SortByIndex);

                segmentToRemove.joint.attachedRigidbody.bodyType = RigidbodyType2D.Static;
                segmentToRemove.joint.enabled = false;
                segmentToRemove.GetComponent<SpriteRenderer>().enabled = false;
                segmentToRemove.isRemoved = true;
                alternateShortenSide = !alternateShortenSide;
                intervalCountForRopeControl = 0;
            }
        }         
    }

    bool alternateLengthenSide;
    public void Lengthen()
    {
        if (removedSegments.Count > 0)
        {
            intervalCountForRopeControl += Time.deltaTime;
            if (intervalCountForRopeControl >= ropeControlInterval)
            {
                RopeSegment segmentToRestore = alternateLengthenSide ? removedSegments[0] : removedSegments[removedSegments.Count - 1];

                segmentToRestore.previousSegment = alternateLengthenSide ? segmentsList[segmentToRestore.index - 1] : segmentsList[segmentToRestore.index + 1];
                segmentToRestore.nextSegment = segmentToRestore.previousSegment.nextSegment;
                segmentToRestore.previousSegment.nextSegment = segmentToRestore;

                segmentToRestore.nextSegment.previousSegment = segmentToRestore;
                segmentToRestore.nextSegment.joint.connectedBody = segmentToRestore.joint.attachedRigidbody;

                segmentToRestore.transform.position = (Vector2)segmentToRestore.previousSegment.transform.position - segmentToRestore.joint.anchor;
                segmentToRestore.transform.rotation = segmentToRestore.previousSegment.transform.rotation;
                segmentToRestore.joint.connectedBody = segmentToRestore.previousSegment.joint.attachedRigidbody;

                removedSegments.Remove(segmentToRestore);
                breakableSegments.Add(segmentToRestore);
                breakableSegments.Sort(RopeSegment.SortByIndex);

                segmentToRestore.joint.attachedRigidbody.bodyType = RigidbodyType2D.Dynamic;
                segmentToRestore.joint.enabled = true;
                segmentToRestore.GetComponent<SpriteRenderer>().enabled = true;
                segmentToRestore.isRemoved = false;
                alternateLengthenSide = !alternateLengthenSide;
                intervalCountForRopeControl = 0;
            }
        }
    }

    public void Break(RopeSegment brokenSegment)
    {
        brokenSegment.joint.enabled = false;
        brokenSegment.isBroken = true;
        this.isBroken = true;
        this.brokenSegment = brokenSegment;
        distanceJoint.enabled = false;
        soundManager.Play(soundManager.breakRope);
        breakParticles.transform.position = brokenSegment.transform.position;
        breakParticles.Play();
    }

    public void CheckIntegrity()
    {
        foreach (RopeSegment segment in segmentsList)
        {
            if (segment.isBroken)
            {
                isBroken = true;                
                return;
            }
        }

        isBroken = false;
        brokenSegment = null;
        distanceJoint.enabled = true;
    }
}
