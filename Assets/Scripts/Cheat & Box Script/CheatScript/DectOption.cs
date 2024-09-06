using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class DetectOption : MonoBehaviour
{
    public enum DetectType
    {
        Up,
        Down,
    };

    [Header("감지 옵션")]
    public DetectType dectType;
    [Header("감지 범위")]
    public float y = 10f; // y축 길이    
    public float xSize; // x축 길이
    [Header("X축 길이 감소비율")]
    [Range(0f, 1f)]
    public float xPercent = 1f; // x축 길이 감소 비율
    public bool isDetected = false; // 감지 여부
    public bool isDetectUp = false; // 위 감지 여부
    public bool isDetectDown = false; // 아래 감지 여부
    [Header("플레이어 레이어")]
    public LayerMask playerLayer; // 플레이어 레이어    
    protected Vector3 adaptPos; // 감지 범위 중심
    
    protected virtual void Update()
    {
        Detect();
    }

    // 감지 범위
    protected virtual void Detect()
    {
        // 위를 감지
        if (dectType == DetectType.Up)
        {
            // 감지 범위 중심으로 하게 하기 위해 
            adaptPos = transform.position + new Vector3(0, y / 2, 0);
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
            adaptPos = transform.position + new Vector3(0, -y / 2, 0);
            // 플레이어가 범위에서 감지되면
            if (Physics2D.OverlapBox(adaptPos, new Vector2(xSize * xPercent, y), 0, playerLayer) && isDetected == false)
            {
                isDetectDown = true;
                isDetected = true;
            }
        }
    }
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // 사각형의 높이를 아래쪽으로만 증가시키기 위해 중심을 아래로 이동
        if (dectType == DetectType.Up)
        {            
            Gizmos.DrawWireCube(adaptPos, new Vector2(xSize * xPercent, y));
        }
        else if (dectType == DetectType.Down)
        {            
            Gizmos.DrawWireCube(adaptPos, new Vector2(xSize * xPercent, y));
        }
    }
#endif
}
