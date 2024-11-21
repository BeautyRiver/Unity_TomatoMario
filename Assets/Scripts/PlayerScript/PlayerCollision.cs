using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

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
        if (MakerManager.instance.isGameMaker)
            return;
        if (playerMove.isDead)
            return;
        GameObject obj = collision.gameObject;
        // Enemy의 머리를 감지했을때
        if (gameObject.CompareTag("Player") && obj.tag == "EnemyHitBox")
        {
            if (playerMove.canKillEnemy) // Enemy 머리를 발 밑에 두고있으면 죽일 수 있음
            {
                playerMove.OnAttack(collision.gameObject);
            }
        }

        // 함정 or Enemy와 충돌시 사망
        if (gameObject.CompareTag("Player") && obj.tag == "Enemy" || obj.CompareTag("Spike"))
        {
            if (playerMove.isBig)
                return;
            playerMove.OnDie();
            Debug.Log("몬스터 충돌");
        }

        if (obj.tag == "Item")
        {
            Destroy(collision.gameObject);
            //transform.DOScale(Vector3.one * 20f, 1.5f).SetEase(Ease.Linear);
            transform.DOScale(Vector3.one * 20f, 1.5f).SetEase(Ease.Linear).OnComplete(() => playerMove.OnDie());
            playerMove.isBig = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (playerMove.isDead)
            return;

        if (!playerMove.isBig)
            return;

        // 충돌한 오브젝트가 타일맵인지 확인
        if (collision.gameObject.TryGetComponent<Tilemap>(out Tilemap tilemap))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 충돌 지점의 월드 좌표 가져오기
                Vector3 contactPosition = contact.point;

                // 타일맵의 위치로 변환
                Vector3Int cellPosition = tilemap.WorldToCell(contactPosition);

                // 해당 위치의 타일을 제거
                if (tilemap.HasTile(cellPosition))
                {
                    tilemap.SetTile(cellPosition, null);
                    Debug.Log($"타일 제거: {cellPosition}");
                }
            }
        }
        else
        {
            collision.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (playerMove.isDead)
            return;

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
