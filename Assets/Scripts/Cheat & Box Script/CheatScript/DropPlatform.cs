using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DropPlatform : DetectOption
{
    [Header("블록 개수")]
    public int blockCount; // 블록 개수
    private Rigidbody2D rb; // 리지드바디
    private RectTransform rectTransform; // 렉트트랜스폼
    private void Awake()
    {
        blockCount = 0; // 블록 개수 초기화
        rectTransform = GetComponent<RectTransform>(); 
        rb = GetComponent<Rigidbody2D>();

        // 자식 블록 개수 세기
        SetRectTransfrom();
    }

    protected override void Update()
    {
        base.Update();
    
        // 위가 감지되면
        if (isDetectUp)
        {
            rb.isKinematic = false; // 리지드바디 활성화
            // 자식 블록들을 트리거로 변경
            foreach (Transform child in transform)
            {
                child.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            isDetectUp = false; // 감지 초기화
        }
        // 아래가 감지되면
        else if (isDetectDown)
        {
            rb.isKinematic = false; // 리지드바디 활성화
            // 자식 블록들을 트리거로 변경 & 태그 변경(부딫히면 죽게)
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "Spike";
                child.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            isDetectDown = false; // 감지 초기화
        }
    }
    // 감지 범위 (오버라이딩)
    protected override void Detect()
    {
        // 위를 감지 (특정 부분 실행하지 않음)
        if (dectType == DetectType.Up)
        {
            // 감지 범위 중심으로 하게 하기 위해 
            adaptPos = transform.position + new Vector3(xSize / 2, y / 2, 0); // 이 부분을 주석 처리 또는 삭제
            // 플레이어가 범위에서 감지되면
            if (Physics2D.OverlapBox(adaptPos, new Vector2(xSize * xPercent, y), 0, playerLayer) && isDetected == false)
            {
                isDetectUp = true;
                isDetected = true;
            }
        }
        // 아래를 감지
        else if (dectType == DetectType.Down)
        {
            // 감지 범위 중심으로 하게 하기 위해
            adaptPos = transform.position + new Vector3(xSize / 2, -y / 2, 0);
            // 플레이어가 범위에서 감지되면
            if (Physics2D.OverlapBox(adaptPos, new Vector2(xSize * xPercent, y), 0, playerLayer) && isDetected == false)
            {
                isDetectDown = true;
                isDetected = true;
            }
        }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        SetRectTransfrom();
    }

    private void SetRectTransfrom()
    {
        // 자식 블록 개수 세기
        blockCount = transform.childCount;
        // LayOutGroup때문에 width사이즈를 설정
        xSize = 1 * blockCount;
    }
}
