using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Rope rope;
    public Body otherBody;
    public Rigidbody2D body;
    public Rigidbody2D headRb;
    public Rigidbody2D leftArm;
    public Rigidbody2D rightArm;
    public Rigidbody2D leftHand;
    public Rigidbody2D rightHand;    
    public Collider2D bodyCollider;
    public SpriteRenderer head;
    public Sprite headSprite1;
    public Sprite headSprite2;
    public LayerMask groundMask;
    public LayerMask otherBodyMask;
    public bool tryHolding;
    public bool isHolding;
    public bool canJump;    
    SoundManager soundManager;

    public delegate void Release();
    public event Release ReleaseEvent;

    private Vector2 moveValue;
    private bool move;
    private void Start()
    {
        changeHeadTimer = Random.Range(1, 10);
        changeDuration = Random.Range(0.2f, 0.5f);
        soundManager = FindObjectOfType<SoundManager>();
    }
    public void Move(Vector2 direction, float xForce, float yForce)
    {
        moveValue = direction * xForce;
        move = true;
    }

    private void FixedUpdate()
    {
        if (move)
        {
            body.AddForce(moveValue);
            if (IsTouchinGroundWithColliders) body.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
            move = false;
        }
    }

    public void Jump(float force)
    {
        if (canJump == false) return;
        body.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        soundManager.Play(soundManager.jump);
        canJump = false;
    }

    public void Hold(bool hold)
    {
        tryHolding = hold;
        if (hold == false) ReleaseEvent();
    }

    float changeHeadTimer;
    float changeDuration;
    public float timer;
    bool changeHead;
    private void Update()
    {
        AnimateFace();
        CheckJumpState();
    }

    private void CheckJumpState()
    {
        if (IsHangingBelow)
        {
            canJump = true;
        }
        else if (IsTouchingGround)
        {
            if (canJump == false && body.velocity.y < -4) soundManager.Play(soundManager.hitGround, 0.5f);
            canJump = true;
        }
        else if (IsTouchingOtherBody && otherBody.IsTouchingGround)
        {
            canJump = true;
        }
        else canJump = false;
    }

    public bool IsHangingBelow
    {
        get
        {
            float distance = (otherBody.body.position - body.position).magnitude;
            return (body.transform.position.y < otherBody.body.transform.position.y && distance > 2 && rope.isBroken == false && (otherBody.canJump || otherBody.isHolding));
        }
    }

    Vector3 leftBound, rightBound;
    public bool IsTouchingGround
    {
        get
        {
            leftBound = bodyCollider.transform.position - new Vector3(bodyCollider.bounds.extents.x, 0, 0);
            rightBound = bodyCollider.transform.position + new Vector3(bodyCollider.bounds.extents.x, 0, 0);
            return (Physics2D.Raycast(leftBound, Vector2.down, 0.6f, groundMask) || Physics2D.Raycast(rightBound, Vector2.down, 0.6f, groundMask));            
        }
    }

    public bool IsTouchinGroundWithColliders
    {
        get
        {
            return (bodyCollider.IsTouchingLayers(groundMask) ||
            leftArm.GetComponent<Collider2D>().IsTouchingLayers(groundMask) || rightArm.GetComponent<Collider2D>().IsTouchingLayers(groundMask) ||
            leftHand.GetComponent<Collider2D>().IsTouchingLayers(groundMask) || rightHand.GetComponent<Collider2D>().IsTouchingLayers(groundMask));
        }
    }

    public bool IsTouchingOtherBody
    {
        get
        {
            return (bodyCollider.IsTouchingLayers(otherBodyMask) ||
            leftArm.GetComponent<Collider2D>().IsTouchingLayers(otherBodyMask) || rightArm.GetComponent<Collider2D>().IsTouchingLayers(otherBodyMask) ||
            leftHand.GetComponent<Collider2D>().IsTouchingLayers(otherBodyMask) || rightHand.GetComponent<Collider2D>().IsTouchingLayers(otherBodyMask));
        }
    }



    private void AnimateFace()
    {
        timer += Time.deltaTime;

        if (!changeHead && timer >= changeHeadTimer)
        {
            head.sprite = headSprite2;
            changeHead = true;
            changeHeadTimer = Random.Range(2, 5);
            timer = 0;
        }
        else if (changeHead && timer >= changeDuration)
        {
            head.sprite = headSprite1;
            changeHead = false;
            timer = 0;
        }
    }

    private void OnDrawGizmos()
    {        
        Gizmos.DrawLine(leftBound, leftBound + Vector3.down * 0.6f);
        Gizmos.DrawLine(rightBound, rightBound + Vector3.down * 0.6f);
    }
}
