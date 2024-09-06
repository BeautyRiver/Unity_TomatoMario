using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Enemy
{
    public Transform wallCheck;

    protected override void Awake() 
    {
        base.Awake();
    }

    private void FixedUpdate() 
    {
        if (!isHit) // 피격 상태가 아니라면
        {
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y); // 이동

            anim.SetInteger("WalkSpeed",rb.velocity.x != 0 ? 1 : 0);

            if (Physics2D.OverlapCircle(wallCheck.position, 0.01f, layerMask)) //  
            {
                MonsterFlip();
            }
        }    
    }
#if UNITY_EDITOR
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(wallCheck.position, 0.01f);
    }
#endif
}
