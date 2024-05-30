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

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            blockCount++;
        }
    }

    private void Update()
    {
        if (dectType == DectType.Up)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(0, y / 2, 0);
            Physics2D.OverlapBox(adjustedPosition, new Vector2(1 * blockCount, y), 0);
        }
        else if
        (dectType == DectType.Down)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(0, -y / 2, 0);
            Physics2D.OverlapBox(adjustedPosition, new Vector2(1 * blockCount, y), 0);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // 사각형의 높이를 아래쪽으로만 증가시키기 위해 중심을 아래로 이동
        if (dectType == DectType.Up)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(0, y / 2, 0);
            Gizmos.DrawWireCube(adjustedPosition, new Vector2(1 * blockCount, y));
        }
        else if (dectType == DectType.Down)
        {
            Vector3 adjustedPosition = transform.position + new Vector3(0, -y / 2, 0);
            Gizmos.DrawWireCube(adjustedPosition, new Vector2(1 * blockCount, y));
        }
    }
}
