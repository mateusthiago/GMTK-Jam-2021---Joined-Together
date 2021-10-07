using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodies : MonoBehaviour
{
    public Rigidbody2D b1, b2;
    public float jumpForce;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void B1Jump()
    {
        b1.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
