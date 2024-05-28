using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerMove pm;
    public float fallWaitTime; // 몸이 커진 후 바닥이 떨어지는 시간
    private void Awake()
    {
        pm = GetComponent<PlayerMove>();
    }

    #region Collision, Trigger 관련        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        // Enemy의 머리를 감지했을때
        if (gameObject.CompareTag("Player") && obj.tag == "EnemyHitBox")
        {
            if (pm.canKillEnemy) // Enemy 머리를 발 밑에 두고있으면 죽일 수 있음
            {
                pm.OnAttack(collision.gameObject);
            }
            else
            {
                pm.OnDie();
            }
        }
        // 함정 or Enemy와 충돌시 사망
        if (gameObject.CompareTag("Player") && obj.tag == "Enemy" || obj.CompareTag("Spike"))
        {
            pm.OnDie();
            Debug.Log("몬스터 충돌");
        }

        if (obj.tag == "Item")
        {
            if (!pm.isMuscle)
            {
                pm.rigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

                pm.EnabledHeadSensor(false);

                gameObject.tag = "Player_muscle";
                transform.localScale = pm.initialScale * 6f;
                // 플레이어의 높이도 증가
                float heightIncrease = (transform.localScale.y - pm.initialScale.y) / 2;
                transform.position = new Vector2(transform.position.x, transform.position.y + heightIncrease);
                //animator.runtimeAnimatorController = muscleAnimatorController;
                pm.isMuscle = true;
            }
            Destroy(collision.gameObject);
        }      
    }
    

    #endregion
}
