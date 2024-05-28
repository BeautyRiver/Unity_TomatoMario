using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMarioPlayerController : MonoBehaviour
{
    CatMarioMovement2D movement;
    private void Awake()
    {
        movement = GetComponent<CatMarioMovement2D>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        movement.Move(x);

        if (Input.GetButtonDown("Jump"))
        {
            movement.Jump();
        }
        
        if (Input.GetButton("Jump"))
        {
            movement.isLongJump = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            movement.isLongJump = false;
        }

    }
    
}
