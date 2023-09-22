using System.Collections;
using UnityEngine;

public class ENEMYTEST : MonoBehaviour
{
    public enum 몬스터
    {
        버섯,
        거미,
        벌,
    };
    [Header("몬스터의 기본 속성")]
    public 몬스터 몬스터속성;
    public int 현재체력 = 1; // 현재 체력
    public float 이동속도 = 5f; // 이동 속도
    public float 점프파워 = 10; // 점프 파워
    public float 공격쿨타임 = 3f; // 공격 쿨타임
    private float atkCoolTimeCalc = 3f; // 쿨타임 계산을 위한 변수

    [Header("몬스터 상태")]
    public bool 피격상태= false; // 피격 상태
    private bool isGround = true; // 지면에 있는지 여부
    private bool 공격가능상태 = true; // 공격 가능 상태
    private bool MonsterDirRight; // 몬스터의 방향(오른쪽을 바라보고 있는지 여부)

    [Header("컴포넌트")]
    protected Rigidbody2D rb; // Rigidbody2D 컴포넌트
    protected BoxCollider2D boxCollider; // BoxCollider2D 컴포넌트
    protected SpriteRenderer spriteRenderer;
    public GameObject hitBoxCollider; // 히트박스
    public Animator Anim; // 애니메이터
    public LayerMask layerMask; // 레이어 마스크(지면 체크 등에 사용)

}