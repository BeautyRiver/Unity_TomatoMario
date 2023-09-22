using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    public Transform[] wallCheck;

    private void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (!isHit)
        {          
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);

            if (!Physics2D.OverlapCircle(wallCheck[0].position, 0.01f, layerMask) &&
                Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask) &&
                 !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), -transform.localScale.x * transform.right, 1f, layerMask))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
            else if (Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask))
            {
                MonsterFlip();
            }
        }

    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(wallCheck[0].position, 0.01f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(wallCheck[1].position, 0.01f);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.1f), -transform.localScale.x * transform.right * 1f);

        if (Physics2D.OverlapCircle(wallCheck[0].position, 0.01f, layerMask))
            Debug.Log("0번 감지");
        if (Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask))
            Debug.Log("1번 감지");
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), -transform.localScale.x * transform.right, 1f, layerMask))
            Debug.Log("레이캐스트 감지");
    }

    /*protected void OnTriggerEnter2D ( Collider2D collision )
    {
        base.OnTriggerEnter2D ( collision );
        if ( collision.transform.CompareTag ( "PlayerHitBox" ) )
        {
            MonsterFlip ( );
        }
    }*/
}
