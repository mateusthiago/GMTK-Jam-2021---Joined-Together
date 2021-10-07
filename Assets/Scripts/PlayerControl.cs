using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Body leftBody;
    public Body rightBody;
    public Rope rope;
    public float moveXForce;
    public float moveYForce;
    public float jumpForce;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.PauseUnpauseGame();
        }
        if (GameManager.instance.isPaused) return;

        if (Input.GetKey(KeyCode.A))
        {
            leftBody.Move(Vector2.left, moveXForce, moveYForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            leftBody.Move(Vector2.right, moveXForce, moveYForce);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            leftBody.Jump(jumpForce);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            leftBody.Hold(true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            leftBody.Hold(false);
        }



        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rightBody.Move(Vector2.left, moveXForce, moveYForce);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rightBody.Move(Vector2.right, moveXForce, moveYForce);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {            
            rightBody.Jump(jumpForce);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            rightBody.Hold(true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            rightBody.Hold(false);
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            rope.PullTogether();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            //rope.Shorten();
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //if (rope.isBroken == false) rope.Lengthen();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            rope.SpontaneousBreak();
        }


        
        

    }
}
