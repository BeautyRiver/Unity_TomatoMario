using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/*#if UNITY_EDITOR
[ExecuteInEditMode]
#endif*/

public class PlayerMove : MonoBehaviour
{
    // 이벤트
    public static event Action<float> OnPlayerJumped; // 점프 이벤트
    public static event Action DownHoleEvent; //  홀 다운 이벤트

    [Header("카메라 및 게임 관리")]
    public Camera playerCamera;

    [Header("오디오")]
    public AudioClip audioJump;
    private AudioSource audioSource;

    [Header("플레이어 속성")]
    public int life = 3; // 생명

    [Header("속도")]
    [SerializeField] private float maxSpeed; // 최대 이동 속도    
    [SerializeField] private float moveSpeed; // 이동 속도

    [Header("점프")]
    public float jumpPower; // 현재 점프 힘
    public float needTimeForLongJump = 1f; // 긴 점프를 위한 시간
    public float shortJumpForce = 9f; // 짧은 점프 힘
    public float longJumpForce = 12f; // 긴 점프 힘
    public float longJumpTimer;


    [Header("위치 관련")]
    public float moveDir; // 이동 방향
    Vector2 fixPos = Vector2.zero; // 고정될 위치
    [HideInInspector]
    public Vector3 initialScale; // 초기 스케일 값
    public Vector2 initPos; // 초기 위치

    [Header("레이어 마스크")]
    public LayerMask platFormLayer; // 플랫폼 레이어
    public LayerMask enemyHitBoxLayer; // 적 레이어

    [Header("상태 검사")]
    public bool isGrounded; // 땅에 닿았는지 여부
    public bool canKillEnemy; // 적을 죽일 수 있는지 여부
    public bool isDead; // 사망 상태
    
    public bool isBig; // 커진 상태
    public bool isDownKey; // 아래키 입력 상태

    [Header("컴포넌트")]
    public GameObject headSensor; // 머리 센서
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider2D; // 박스 콜라이더

    [Header("감지 센서 크기")]
    [SerializeField] private Vector2 groundCheckBoxSize = new Vector2(0.43f, 0.18f);
    [SerializeField] private Vector2 enemyCheckBoxSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 footPosition; // 발 위치
    [SerializeField] private float refVelocity;

    [Range(0f, 1f)]
    [SerializeField] private float slideRate;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void InitPlayer()
    {
        gameObject.SetActive(true);
        isDead = false;
        boxCollider2D.enabled = true;
        rb.gravityScale = 1;
        jumpPower = shortJumpForce;
        initialScale = transform.localScale;
        initPos = transform.position;
        float x = PlayerPrefs.GetFloat("SavePointX", transform.position.x);
        float y = PlayerPrefs.GetFloat("SavePointY", transform.position.y);        
        transform.position = new Vector2(x, y);
        Camera.main.transform.position = new Vector3(transform.position.x, -1.7f, -10);
        life = PlayerPrefs.GetInt("PlayerLife", 3); // PlayerPrefs에서 life 값을 불러오기 
    }

    public void MakerMode()
    {
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        boxCollider2D.enabled = false;
    }
    void Update()
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (isDead)
            return;
        
        PlayerKeyDown(); // 플레이어 키 입력
        PlayerAnmation(); // 플레이어 애니메이션
        CanMovePos(); // 카메라의 왼쪽이상으로 이동못하게        
        CheckGroundAndEnemy(); // 바닥 충돌 체크
        GroundFriction(); // 바닥 마찰력       

    }

    void FixedUpdate()
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (isDead)
            return;
        
        PlayerMoveMent(); // 플레이어 이동
        HeadSensorControll(); // 머리 센서 컨트롤
    }

    private void PlayerMoveMent()
    {
        // 이동
        rb.AddForce(new Vector2(moveDir * moveSpeed, 0), ForceMode2D.Force);

        // 최대 속도 제한
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            // rb.velocity.y는 그대로 유지하면서 x축 속도만 조정
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }

        // 플레이어 최대 점프 제한
        if (rb.velocity.y > longJumpForce)
            rb.velocity = new Vector2(rb.velocity.x, longJumpForce);
    }

    private void PlayerAnmation()
    {
        // PlayerFlip
        if (moveDir != 0) // 움직이고 있을때
        {
            float newScaleX = Mathf.Sign(moveDir) * Mathf.Abs(transform.localScale.x); // Sign은 부호를 반환하는 함수 (양수면 1, 음수면 -1 0이면 0)
            transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z); // 방향전환
        }
        animator.SetBool("isWalking", moveDir != 0); // 걷기 애니메이션
        animator.SetBool("isFalling", !isGrounded && rb.velocity.y < 0); // 낙하 애니메이션
        animator.SetBool("isJumping", !isGrounded && rb.velocity.y > 0); // 점프 애니메이션
    }

    private void PlayerKeyDown()
    {
        // 방향전환 및 이동
        moveDir = Input.GetAxisRaw("Horizontal");
        // 점프
        if (Input.GetButtonUp("Jump") && isGrounded)
        {
            if (longJumpTimer > needTimeForLongJump) // 점프를 꾹 누르면            
            {
                jumpPower = longJumpForce;
            }
            else
            {
                jumpPower = shortJumpForce;
            }
            longJumpTimer = 0f; // 타이머 초기화
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // 점프                   
            audioSource.clip = audioJump; // 오디오 클립 설정
            GameManager.instance.SoundOn("jump"); // 점프 사운드

            // 이벤트 호출
            OnPlayerJumped?.Invoke(jumpPower);

            jumpPower = shortJumpForce;
        }

        // 점프 파워 차징
        if (Input.GetButton("Jump") && isGrounded)
        {
            longJumpTimer += Time.deltaTime;
        }

        // DownKey를 눌렀는지 여부
        //if (Input.GetKeyDown(KeyCode.DownArrow) && )
    }

    // Head 센서 컨트롤
    private void HeadSensorControll()
    {
        // 현재 jump anmation이 실행중이면
        if (animator.GetBool("isJumping"))
        {
            // 머리 센서 활성화
            headSensor.SetActive(true);
        }
        else
        {
            // 머리 센서 비활성화
            headSensor.SetActive(false);
        }
    }


    // 카메라의 왼쪽이상으로 이동못하게
    private void CanMovePos()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);

        if (screenPoint.x < 0f)
            screenPoint.x = 0f;

        if (screenPoint.y < -8f)
            OnDie();

        transform.position = Camera.main.ViewportToWorldPoint(screenPoint);
    }

    //몬스터 공격할때
    public void OnAttack(GameObject enemy)
    {
        if (isDead)
            return;

        //적 사망
        Enemy monster = enemy.GetComponentInParent<Enemy>();
        monster.gameObject.tag = "DeadEnemy"; // 죽은 몬스터 태그 변경
        monster.OnEnemyDie(); // 몬스터 사망

        //밟았을때 위로 점프되기
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * shortJumpForce, ForceMode2D.Impulse);

        //점수증가
        GameManager.instance.stagePoint += 100;
    }

    //Player 사망
    public void OnDie()
    {
        isDead = true;
        //Sprite 콜라이더 끄기
        boxCollider2D.enabled = false;

        // EnabledHeadSensor(false);

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        gameObject.layer = LayerMask.NameToLayer("PlayerDie");
        life--;
        PlayerPrefs.SetInt("PlayerLife", life); // life 값을 PlayerPrefs에 저장합니다.
        PlayerPrefs.Save();
        GameManager.instance.SoundOn("death");

        StartCoroutine(DieMotion());


    }
    // 몬스터 공격, 땅 체크 확인
    private void CheckGroundAndEnemy()
    {
        Bounds bounds = boxCollider2D.bounds; // 박스 콜라이더의 경계
        footPosition = new Vector2(bounds.center.x, bounds.min.y); // 발 위치

        isGrounded = Physics2D.BoxCast(footPosition, groundCheckBoxSize, 0, Vector2.down, 0.01f, platFormLayer) ? true : false; // 땅에 닿았는지 여부
        canKillEnemy = Physics2D.BoxCast(footPosition, enemyCheckBoxSize, 0, Vector2.down, 0.01f, enemyHitBoxLayer) ? true : false; // 적을 죽일 수 있는지 여부
    }

    // 토관에 들어갈때
    public void InGreenHole(Vector2 pos)
    {
        isDead = true;
        boxCollider2D.enabled = false;
        fixPos = new Vector2(pos.x, transform.position.y);
        rb.velocity = Vector2.zero;
        animator.SetBool("inGreenHole", true);
        rb.gravityScale = 0;
        boxCollider2D.enabled = false;
        StartCoroutine(GreenHoleMoveDown());
    }

    // 바닥 마찰력
    private void GroundFriction()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(moveDir) <= 0.01f) // 이동중이 아닐때
                rb.velocity = new Vector2(Mathf.SmoothDamp(rb.velocity.x, 0f, ref refVelocity, slideRate), rb.velocity.y); // 부드러운 감속
        }
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }
    #region  코루틴
    //죽을때 마리오처럼 죽게하는 모션 코루틴
    IEnumerator DieMotion()
    {
        boxCollider2D.enabled = false;
        animator.SetTrigger("doDying");
        yield return new WaitForSeconds(0.3f);
        rb.AddForce(Vector2.up * longJumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.7f);
        rb.gravityScale = 8;
        //Sprite 삭제
        StartCoroutine(DeActive(1));
    }

    // 플레이어 비활성화
    IEnumerator DeActive(float time)
    {
        yield return new WaitForSeconds(time);
        //gameObject.SetActive(false);
        GameManager.instance.DeathSceneOn(); // 데쓰씬 키기
    }

    // 토관 들어가는 모션
    IEnumerator GreenHoleMoveDown()
    {
        float moveTime = 0;
        float moveSpeed = 1.4f;
        while (moveTime < 2f)
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            moveTime += Time.deltaTime;
            yield return null;
        }
        animator.SetBool("inGreenHole", false);
        OnDie();
    }
    #endregion

    #region 디버그
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        if (boxCollider2D != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(footPosition + Vector2.down * 0.01f, enemyCheckBoxSize);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(footPosition + Vector2.down * 0.01f, groundCheckBoxSize);
        }

        //     if (isGrounded)
        //     {
        //         Debug.Log("땅에 닿음");
        //     }
        //     if (canKillEnemy)
        //     {
        //         Debug.Log("적을 죽일 수 있음");
        //     }

    }
#endif

    #endregion
}
