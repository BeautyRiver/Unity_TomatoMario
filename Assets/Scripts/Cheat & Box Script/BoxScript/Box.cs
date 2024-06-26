using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]  //<- 이 부분이 에디터에서 작동되도록 선언하는 부분
#endif
public class Box : MonoBehaviour
{
    [Header("센서")]
    public float yOffset = 0f; // 센서 위치    
    public Vector2 seonsorSize = new Vector2(1, 1); // 센서 크기

    [Header("플레이어 레이어")]
    public LayerMask playerLayer; // 플레이어 레이어

    [SerializeField] protected bool isBoxOn = false; // 박스가 켜져있는지
    protected BoxCollider2D boxCollider2D;
    protected SpriteRenderer spriteRenderer;
    protected Bounds bounds;

    protected virtual void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        bounds = spriteRenderer.bounds; // 박스의 바운드         
        // 플레이어가 센서에 닿았을 때
        if (Physics2D.OverlapBox(new Vector2(bounds.center.x, bounds.min.y + yOffset), seonsorSize, 0, playerLayer) && isBoxOn == false)
        {
            isBoxOn = true;
            Debug.Log("플레이어가 센서에 닿았다.");
        }
        else
        {
            isBoxOn = false;
        }
    }

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(bounds.center.x, bounds.min.y + yOffset), seonsorSize);
    }
#endif
}
