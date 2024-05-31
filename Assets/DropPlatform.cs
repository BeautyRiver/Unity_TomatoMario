using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DropPlatform : MonoBehaviour
{
    public enum DectType
    {
        Up,
        Down,
    };

    public DectType dectType;
    public float y = 10f;
    public int blockCount = 0;
    [Range(0f, 1f)]
    public float xPercent = 1f;
    public bool isDeceted = false;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private RectTransform rectTransform;
    private float sizeDeltaBlock;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
        foreach (Transform child in transform)
        {
            blockCount++;
        }
        rectTransform.sizeDelta = new Vector2(sizeDeltaBlock, 1);
        sizeDeltaBlock = 1 * blockCount;
    }

    private void Update()
    {
        // 위를 감지
        if (dectType == DectType.Up)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(sizeDeltaBlock * 0.5f, y / 2, 0);
            if (Physics2D.OverlapBox(adjustedPosition, new Vector2(sizeDeltaBlock * xPercent, y), 0, playerLayer) && isDeceted == false)
            {
                rb.isKinematic = false;
                foreach (Transform child in transform)
                {
                    child.GetComponent<BoxCollider2D>().isTrigger = true;
                }
                isDeceted = true;
            }
        }
        // 아래를 감지
        else if (dectType == DectType.Down)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(sizeDeltaBlock * 0.5f, -y / 2, 0);
            if (Physics2D.OverlapBox(adjustedPosition, new Vector2(sizeDeltaBlock * xPercent, y), 0, playerLayer) && isDeceted == false)
            {
                rb.isKinematic = false;
                foreach (Transform child in transform)
                {
                    child.gameObject.tag = "Spike";
                    child.GetComponent<BoxCollider2D>().isTrigger = true;

                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // 사각형의 높이를 아래쪽으로만 증가시키기 위해 중심을 아래로 이동
        if (dectType == DectType.Up)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(sizeDeltaBlock * 0.5f, y / 2, 0);
            Gizmos.DrawWireCube(adjustedPosition, new Vector2(sizeDeltaBlock * xPercent, y));
        }
        else if (dectType == DectType.Down)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(sizeDeltaBlock * 0.5f, -y / 2, 0);
            Gizmos.DrawWireCube(adjustedPosition, new Vector2(sizeDeltaBlock * xPercent, y));
        }
    }
}
