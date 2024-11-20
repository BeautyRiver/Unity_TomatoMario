using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class DropPlatform_Single : DetectOption
{
    public int rowCount; // 행 개수
    public GameObject defaultBlock; // 블록

    private Rigidbody2D rb; // 리지드바디
    private RectTransform rectTransform; // 렉트트랜스폼
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
        CreateRowBlocks();
    }

    private void CreateRowBlocks()
    {
        // 자식 오브젝트 삭제
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }        

        // 블록 생성
        for (int i = 0; i < rowCount; i++)
        {
            GameObject block = Instantiate(defaultBlock, transform);
        }
    }

    protected override void DetectTrigger_Up()
    {
        rb.isKinematic = false; // 리지드바디 활성화         
        isDetectUp = false; // 감지 초기화

    }
    protected override void DetectTrigger_Down()
    {
        rb.isKinematic = false; // 리지드바디 활성화
        gameObject.tag = "Spike";
        GetComponent<BoxCollider2D>().isTrigger = true;
        isDetectDown = false; // 감지 초기화
    }
}
