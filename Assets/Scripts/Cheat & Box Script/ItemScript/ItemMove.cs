using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public float speed;
    public Transform wallCheck;
    public LayerMask platformLayer;
    private int dir = 1;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        rb.velocity = new Vector2(transform.localScale.x * speed, rb.velocity.y);

        if (Physics2D.OverlapCircle(wallCheck.position, 0.01f, platformLayer))
        {
            Flip();
        }
    }

    //  방향 전환 함수
    protected void Flip()
    {
        dir *= -1; // 방향 전환

        Vector3 thisScale = transform.localScale;
        // 스케일을 반전시켜 방향 전환

        if (dir == 1)
            thisScale.x = Mathf.Abs(thisScale.x);
        else
            thisScale.x = -Mathf.Abs(thisScale.x);

        transform.localScale = thisScale;
        //rb.velocity = Vector2.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(wallCheck.position, 0.01f);
    }
#endif
}
