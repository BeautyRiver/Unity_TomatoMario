using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    public Transform[] wallCheck;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (!isHit) // 피격 상태가 아니라면
        {          
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y); // 이동
            
            // 벽체크
            if (!Physics2D.OverlapCircle(wallCheck[0].position, 0.01f, layerMask) && // 위쪽 벽 체크 (Top) 위쪽 벽은 없어야 함
                Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask) && // 아래쪽 벽 체크 (Bottom)
                 !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), -transform.localScale.x * transform.right, 1f, layerMask)) // 레이캐스트 체크 (발 밑)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
            else if (Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask)) //  
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

        // if (Physics2D.OverlapCircle(wallCheck[0].position, 0.01f, layerMask))
        //     Debug.Log("0번 감지");
        // if (Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask))
        //     Debug.Log("1번 감지");
        // if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.1f), -transform.localScale.x * transform.right, 1f, layerMask))
        //     Debug.Log("레이캐스트 감지");
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
