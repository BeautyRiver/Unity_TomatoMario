﻿using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터의 기본 속성")]
    public int currentHp = 1; // 현재 체력
    public float moveSpeed = 5f; // 이동 속도
    public float jumpPower = 10; // 점프 파워
    public float atkCoolTime = 3f; // 공격 쿨타임
    public float atkCoolTimeCalc = 3f; // 쿨타임 계산을 위한 변수

    [Header("몬스터 상태")]
    public bool isHit = false; // 피격 상태
    public bool isGround = true; // 지면에 있는지 여부
    public bool canAtk = true; // 공격 가능 상태
    public bool MonsterDirRight; // 몬스터의 방향(오른쪽을 바라보고 있는지 여부)

    [Header("컴포넌트")]
    protected Rigidbody2D rb; // Rigidbody2D 컴포넌트
    protected BoxCollider2D boxCollider; // BoxCollider2D 컴포넌트
    protected SpriteRenderer spriteRenderer;
    public GameObject hitBoxCollider; // 히트박스
    public Animator Anim; // 애니메이터
    public LayerMask layerMask; // 레이어 마스크(지면 체크 등에 사용)


    protected void Awake()
    {
        // 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 코루틴 시작
        StartCoroutine(CalcCoolTime());
        /*StartCoroutine ( ResetCollider ( ) );*/
    }

    // 히트박스 리셋 코루틴
    IEnumerator ResetCollider()
    {
        while (true)
        {
            yield return null;
            if (!hitBoxCollider.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }

    // 공격 쿨타임 계산 코루틴
    IEnumerator CalcCoolTime()
    {
        while (true)
        {
            yield return null;
            if (!canAtk)
            {
                atkCoolTimeCalc -= Time.deltaTime;
                if (atkCoolTimeCalc <= 0)
                {
                    atkCoolTimeCalc = atkCoolTime;
                    canAtk = true;
                }
            }
        }
    }

    // 특정 애니메이션이 재생 중인지 확인하는 함수
    public bool IsPlayingAnim(string AnimName)
    {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName))
        {
            return true;
        }
        return false;
    }

    // 애니메이션 트리거 설정 함수
    public void MyAnimSetTrigger(string AnimName)
    {
        if (!IsPlayingAnim(AnimName))
        {
            Anim.SetTrigger(AnimName);
        }
    }

    // 몬스터 방향 전환 함수
    protected void MonsterFlip()
    {
        MonsterDirRight = !MonsterDirRight;

        Vector3 thisScale = transform.localScale;
        // 스케일을 반전시켜 방향 전환

        if (MonsterDirRight)
            thisScale.x = -Mathf.Abs(thisScale.x);
        else
            thisScale.x = Mathf.Abs(thisScale.x);

        transform.localScale = thisScale;
        rb.velocity = Vector2.zero;
    }

    // 플레이어가 몬스터 기준 어디 방향에 있는지 찾는 함수
    protected bool IsPlayerDir()
    {
        if (transform.position.x < PlayerData.Instance.Player.transform.position.x ? MonsterDirRight : !MonsterDirRight)
        {
            return true;
        }
        return false;
    }

    // 지면 체크 함수
    protected void GroundCheck()
    {
        if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.size, 0, Vector2.down, 0.05f, layerMask))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    // 데미지를 받는 함수
    public void TakeDamage(int dam)
    {
        currentHp -= dam;
        isHit = true;
        // Knock Back or Dead
        hitBoxCollider.SetActive(false);
    }
    public void OnEnemyDie()
    {
        hitBoxCollider.SetActive(false);
        isHit = true;
        //Sprite 콜라이더 끄기
        boxCollider.enabled = false;

        //Sprite 데미지
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite 반전
        spriteRenderer.flipY = true;
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //Sprite 삭제
        Destroy(gameObject, 4f);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        //if ( collision.transform.CompareTag ( ?? ) )
        //{
        //TakeDamage ( 0 );
        //}
    }
}
