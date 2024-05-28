using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
[ExecuteInEditMode]  //<- 이 부분이 에디터에서 작동되도록 선언하는 부분
#endif


public class PlayerMove : MonoBehaviour
{
    // 이벤트
    public static event Action<float> OnPlayerJumped; // 점프 이벤트
    public static event Action DownHoleEvent; //  홀 다운 이벤트

    [Header("카메라 및 게임 관리")]
    public Camera playerCamera;
    public GameManager gameManager;

    [Header("오디오")]
    public AudioClip audioJump;
    private AudioSource audioSource;

    [Header("플레이어 속성")]
    [SerializeField] private float maxSpeed = 5.0f; // 최대 이동 속도
    [SerializeField] private float moveSpeed = 3.0f; // 이동 속도

    [SerializeField] private float jumpPower; // 현재 점프 힘
    public float minJumpPower = 9f; // 최소 점프 힘
    public float maxJumpPower = 12f; // 최대 점프 힘
    public float attackJumpPower; // 점프 후 수직반동
    private float jumpDelay = 0.5f; // 점프 딜레이
    private float lastJumpTime; // 마지막 점프 시간

    public int life = 3; // 생명

    [HideInInspector] public Vector3 initialScale; // 초기 스케일 값
    public Vector2 initPos; // 초기 위치

    public float moveDir; // 이동 방향

    [Header("UI")]
    public Slider jumpBar; // 점프 파워 바

    [Header("위치 고정")]
    Vector2 fixPos = Vector2.zero; // 고정될 위치
    private bool isFixPos = false; // 위치 고정 여부


    [Header("레이어 마스크")]
    public LayerMask platFormLayer; // 플랫폼 레이어
    public LayerMask enemyHitBoxLayer; // 적 레이어

    [Header("상태 검사")]
    private bool isChange = false; // 캐릭터를 변경하였는지 (새 캐릭터)
    public bool isGrounded = false; // 땅에 닿았는지 여부
    public bool canKillEnemy = false; // 적을 죽일 수 있는지 여부
    public bool isMuscle = false; // 근육 상태 여부
    public bool moveOk = true; // 이동 가능 여부
    public bool isDead = false; // 사망 상태
    public bool isDownKey = false; // 아래키 입력 상태
    [HideInInspector] public bool isRbOnRunning = false; // 중력 코루틴 실행 여부

    [Header("컴포넌트")]
    [HideInInspector] public Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider2D; // 박스 콜라이더
    public RuntimeAnimatorController playerAnimatorController; // 플레이어 애니메이터
    public RuntimeAnimatorController muscleAnimatorController; // 근육 상태 애니메이터 컨트롤러
    public RuntimeAnimatorController birdAnimatorController; // 새 애니메이터 *변신

    [Header("감지 센서 크기")]
    [SerializeField] private Vector2 groundCheckBoxSize = new Vector2(0.43f, 0.18f);
    [SerializeField] private Vector2 enemyCheckBoxSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 footPosition; // 발 위치
    [SerializeField] private float refVelocity;
    [SerializeField] private float slideRate;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        // 초기 설정
        jumpPower = minJumpPower;
        jumpBar.value = minJumpPower;
        initialScale = transform.localScale;
        initPos = transform.position;

        float x = PlayerPrefs.GetFloat("SavePointX", transform.position.x);
        float y = PlayerPrefs.GetFloat("SavePointY", transform.position.y);
        float z = PlayerPrefs.GetFloat("SavePointZ", transform.position.z);

        transform.position = new Vector3(x, y, z);

        life = PlayerPrefs.GetInt("PlayerLife", 3); // PlayerPrefs에서 life 값을 불러오기 
        isDead = false;
    }

    void Update()
    {
        //MoveOk가 true일때만
        if (moveOk && isChange == false)
        {
            // 방향전환 및 이동
            moveDir = Input.GetAxisRaw("Horizontal");
            
            if (moveDir != 0)
            {
                float newScaleX = Mathf.Sign(moveDir) * Mathf.Abs(transform.localScale.x); // Sign은 부호를 반환하는 함수 (양수면 1, 음수면 -1 0이면 0)
                transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z); // 방향전환
            }
            // 애니메이션
            animator.SetBool("isWalking", moveDir != 0);

            bool isFalling = rigid.velocity.y < 0.01f;
            if (isFalling)
            {
                animator.SetBool("isJumping", !isGrounded);
            }

            #region 키 다운 체크            
            // 캐릭터 전환
            if (Input.GetKeyDown(KeyCode.Tab)) // 탭을 누르고, 아직 변경(새)가 안된 상태이면
            {
                rigid.velocity = Vector3.zero;
                if (isChange == false)
                {
                    isChange = true; // 변신 완료
                    animator.runtimeAnimatorController = birdAnimatorController;
                }
                else if (isChange == true)
                {
                    isChange = false;
                    animator.runtimeAnimatorController = playerAnimatorController;
                }
            }
            // 점프
            if (Input.GetButtonUp("Jump"))
            {
                if (!animator.GetBool("isJumping") && isGrounded && Time.time > lastJumpTime + jumpDelay)
                {
                    EnabledHeadSensor(true);
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    animator.SetBool("isJumping", true);
                    audioSource.clip = audioJump;
                    gameManager.SoundOn("jump");
                    lastJumpTime = Time.time;

                    // 이벤트 호출
                    OnPlayerJumped?.Invoke(jumpPower);
                }
                jumpPower = minJumpPower;
            }

            // 점프 파워 차징
            if (Input.GetButton("Jump"))
            {
                if (jumpPower <= maxJumpPower)
                {
                    jumpPower += 20f * Time.deltaTime;
                }
            }

            // DownKey를 눌렀는지 여부
            isDownKey = Input.GetKey(KeyCode.DownArrow) ? true : false;

            #endregion

            if (transform.position.y < -7.5f && !(gameObject.layer == LayerMask.NameToLayer("PlayerDie")))
                OnDie();

            // 카메라의 왼쪽이상으로 이동못하게
            // CanMovePos();

            jumpBar.value = jumpPower;
        }
        
        // 새로 캐릭터가 바뀌었을때
        if (moveOk && isChange == true)
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                rigid.velocity = Vector2.up * 3.5f;
            }
        }
        
        // 바닥 충돌 체크
        CheckGroundAndEnemy();
        GroundFriction();

    }
    void FixedUpdate()
    {        
        if (moveOk && isChange == false)
        {
            rigid.AddForce(new Vector2(moveDir * moveSpeed, 0),ForceMode2D.Force);
            if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
            {
                // rigid.velocity.y는 그대로 유지하면서 x축 속도만 조정
                rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);
            }

            if (rigid.velocity.y > maxJumpPower)
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpPower);            
        }
    }

    // 카메라의 왼쪽이상으로 이동못하게
    private void CanMovePos()
    {
        Vector3 screenPoint = playerCamera.WorldToViewportPoint(transform.position);

        if (screenPoint.x < 0f)
            screenPoint.x = 0f;

        transform.position = Camera.main.ViewportToWorldPoint(screenPoint);
    }


    //몬스터 공격할때
    public void OnAttack(GameObject enemy)
    {
        //적 사망
        Enemy monster = enemy.GetComponentInParent<Enemy>();
        monster.gameObject.tag = "DeadEnemy";
        monster.OnEnemyDie();     

        //밟았을때 위로 점프되기
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * attackJumpPower, ForceMode2D.Impulse);


        //점수증가
        gameManager.stagePoint += 100;

    }

    //Player 사망
    public void OnDie()
    {
        if (!isDead)
        {
            //Sprite 콜라이더 끄기
            boxCollider2D.enabled = false;

            EnabledHeadSensor(false);

            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
            moveOk = false;
            gameObject.layer = LayerMask.NameToLayer("PlayerDie");
            life--;
            PlayerPrefs.SetInt("PlayerLife", life); // life 값을 PlayerPrefs에 저장합니다.
            PlayerPrefs.Save();
            gameManager.SoundOn("death");

            StartCoroutine(DieMotion());
        }

    }
    //죽을때 마리오처럼 죽게하는 모션 코루틴
    IEnumerator DieMotion()
    {
        float speed = 8;
        animator.SetTrigger("doDying");
        isDead = true;
        yield return new WaitForSeconds(0.3f);
        rigid.gravityScale = 1;
        rigid.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.7f);
        rigid.gravityScale = 8;
        //Sprite 삭제
        StartCoroutine(DeActive(1));
    }

    // 플레이어 비활성화
    IEnumerator DeActive(float time)
    {
        yield return new WaitForSeconds(time);
        //gameObject.SetActive(false);
        gameManager.DeathSceneOn(); // 데쓰씬 키기
    }

    // 몬스터 공격, 땅 체크 확인
    private void CheckGroundAndEnemy()
    {
        Bounds bounds = boxCollider2D.bounds;
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        canKillEnemy = Physics2D.OverlapBox(footPosition, enemyCheckBoxSize, 0, enemyHitBoxLayer); // 발 밑에 적의 머리가 있으면 죽일 수 있음
        //isGrounded = Physics2D.OverlapBox(footPosition, groundCheckBoxSize, 0, platFormLayer);

        isGrounded = Physics2D.BoxCast(footPosition, groundCheckBoxSize , 0, Vector2.down,0.01f, platFormLayer) ?  true : false;
        canKillEnemy = Physics2D.BoxCast(footPosition, enemyCheckBoxSize, 0, Vector2.down, 0.01f, enemyHitBoxLayer) ? true : false;        
        EnabledHeadSensor(!isGrounded); // 점프할때만 머리 센서 키기
    }

    // 토관에 들어갈때
    public void InGreenHole(Vector2 pos)
    {
        boxCollider2D.enabled = false;
        moveOk = false;
        fixPos = new Vector2(pos.x, transform.position.y);
        rigid.velocity = Vector2.zero;
        animator.SetBool("inGreenHole", true);
        rigid.gravityScale = 0;
        boxCollider2D.enabled = false;
        StartCoroutine(GreenHoleMoveDown());
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

    // 머리 센서끄기
    public void EnabledHeadSensor(bool enable)
    {
        // 모든 자식 오브젝트들 중에서 'HeadSensor' 태그를 가진 오브젝트를 찾음
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player_Head"))
            {
                BoxCollider2D headSensor = child.GetComponent<BoxCollider2D>();
                headSensor.enabled = enable;
                return;
            }
        }
    }

    void GroundFriction()
    {
        if (isGrounded)
        {
            if(Mathf.Abs(moveDir) <= 0.01f)
                rigid.velocity = new Vector2 (Mathf.SmoothDamp(rigid.velocity.x, 0f, ref refVelocity, slideRate), rigid.velocity.y);
        }
    }

    #region 디버그
    // 기즈모 그리기
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(footPosition, groundCheckBoxSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(footPosition, enemyCheckBoxSize);

        }

        if (isGrounded)
        {
            Debug.Log("땅에 닿음");
        }
        if (canKillEnemy)
        {
            Debug.Log("적을 죽일 수 있음");
        }
    }
#endif

#endregion
}
