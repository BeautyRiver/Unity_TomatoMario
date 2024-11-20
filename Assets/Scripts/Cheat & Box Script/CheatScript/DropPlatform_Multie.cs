using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DropPlatform_Multie : DetectOption
{
    [Header("��� ����")]
    public int blockCount; // ��� ����
    private Rigidbody2D rb; // ������ٵ�
    private RectTransform rectTransform; // ��ƮƮ������
    private void Awake()
    {
        blockCount = 0; // ��� ���� �ʱ�ȭ
        rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();

        // �ڽ� ��� ���� ����
        SetRectTransfrom();
    }

    protected override void Update()
    {
        base.Update();

        // ���� �����Ǹ�
        if (isDetectUp)
        {
            rb.isKinematic = false; // ������ٵ� Ȱ��ȭ
            // �ڽ� ��ϵ��� Ʈ���ŷ� ����
            foreach (Transform child in transform)
            {
                child.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            isDetectUp = false; // ���� �ʱ�ȭ
        }
        // �Ʒ��� �����Ǹ�
        else if (isDetectDown)
        {
            rb.isKinematic = false; // ������ٵ� Ȱ��ȭ
            // �ڽ� ��ϵ��� Ʈ���ŷ� ���� & �±� ����(�΋H���� �װ�)
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "Spike";
                child.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            isDetectDown = false; // ���� �ʱ�ȭ
        }
    }
    // ���� ���� (�������̵�)
    protected override void Detect()
    {
        // ���� ���� (Ư�� �κ� �������� ����)
        if (dectType == DetectType.Up)
        {
            // ���� ���� �߽����� �ϰ� �ϱ� ���� 
            adaptPos = transform.position + new Vector3(xSize / 2, detectionY / 2, 0); // �� �κ��� �ּ� ó�� �Ǵ� ����
            // �÷��̾ �������� �����Ǹ�
            if (Physics2D.OverlapBox(adaptPos, new Vector2(xSize * xPercent, detectionY), 0, playerLayer) && isDetected == false)
            {
                isDetectUp = true;
                isDetected = true;
            }
        }
        // �Ʒ��� ����
        else if (dectType == DetectType.Down)
        {
            // ���� ���� �߽����� �ϰ� �ϱ� ����
            adaptPos = transform.position + new Vector3(xSize / 2, -detectionY / 2, 0);
            // �÷��̾ �������� �����Ǹ�
            if (Physics2D.OverlapBox(adaptPos, new Vector2(xSize * xPercent, detectionY), 0, playerLayer) && isDetected == false)
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
        // �ڽ� ��� ���� ����
        blockCount = transform.childCount;
        // LayOutGroup������ width����� ����
        xSize = 1 * blockCount;
    }
}