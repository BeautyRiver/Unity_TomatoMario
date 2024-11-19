using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using DG.Tweening;

public class PlayerCollision : MonoBehaviour
{
    private PlayerMove playerMove;
    public float fallWaitTime; // 몸이 커진 후 바닥이 떨어지는 시간
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    #region Collision, Trigger 관련        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        // Enemy의 머리를 감지했을때
        if (gameObject.CompareTag("Player") && obj.tag == "EnemyHitBox")
        {
            if (playerMove.canKillEnemy) // Enemy 머리를 발 밑에 두고있으면 죽일 수 있음
            {
                playerMove.OnAttack(collision.gameObject);
            }
            else
            {
                playerMove.OnDie();
            }
        }
        // 함정 or Enemy와 충돌시 사망
        if (gameObject.CompareTag("Player") && obj.tag == "Enemy" || obj.CompareTag("Spike"))
        {
            playerMove.OnDie();
            Debug.Log("몬스터 충돌");
        }

        if (obj.tag == "Item")
        {
            Destroy(collision.gameObject);
            transform.DOScale(Vector3.one * 15f, 3f).SetEase(Ease.OutBack).OnComplete(() => playerMove.OnDie());
            playerMove.isBig = true;
        }      
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Spike"))
        {
            playerMove.OnDie();
            Debug.Log("몬스터 충돌");
        }

        if (other.CompareTag("Cloud"))
        {
            playerMove.OnDie();
            other.gameObject.GetComponent<Cloud>().spriteRenderer.sprite = other.gameObject.GetComponent<Cloud>().attackSprite;
        }
    }
    
    #endregion
}
