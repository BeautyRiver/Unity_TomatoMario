using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMarioMovement2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    public bool isLongJump = false; // 꾹 누르면 높게 점프
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();        
    }

    private void FixedUpdate() 
    {
        if (isLongJump == true)    
        {
            rb.gravityScale = 1.5f;
        }
        else if (isLongJump == false)
        {
            rb.gravityScale = 2.5f;
        }
    }

    public void Move(float x)
    {
        rb.velocity = new Vector2(x * speed, rb.velocity.y);
        Debug.Log("이동중");
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

}
