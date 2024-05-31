using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
[ExecuteInEditMode]  //<- �� �κ��� �����Ϳ��� �۵��ǵ��� �����ϴ� �κ�
#endif


public class PlayerMove : MonoBehaviour
{
    // �̺�Ʈ
    public static event Action<float> OnPlayerJumped; // ���� �̺�Ʈ
    public static event Action DownHoleEvent; //  Ȧ �ٿ� �̺�Ʈ

    [Header("ī�޶� �� ���� ����")]
    public Camera playerCamera;
    public GameManager gameManager;

    [Header("�����")]
    public AudioClip audioJump;
    private AudioSource audioSource;

    [Header("�÷��̾� �Ӽ�")]
    [SerializeField] private float maxSpeed = 5.0f; // �ִ� �̵� �ӵ�
    [SerializeField] private float moveSpeed = 3.0f; // �̵� �ӵ�

    [SerializeField] private float jumpPower; // ���� ���� ��
    private float minJumpPower = 6f; // �ּ� ���� ��
    private float maxJumpPower = 12f; // �ִ� ���� ��
    public float attackJumpPower; // ���� �� �����ݵ�
    private float jumpDelay = 0.5f; // ���� ������
    private float lastJumpTime; // ������ ���� �ð�

    public int life = 3; // ����

    [HideInInspector] public Vector3 initialScale; // �ʱ� ������ ��
    public Vector2 initPos; // �ʱ� ��ġ

    public float moveDir; // �̵� ����

    [Header("UI")]
    public Slider jumpBar; // ���� �Ŀ� ��

    [Header("��ġ ����")]
    Vector2 fixPos = Vector2.zero; // ������ ��ġ
    private bool isFixPos = false; // ��ġ ���� ����


    [Header("���̾� ����ũ")]
    public LayerMask platFormLayer; // �÷��� ���̾�
    public LayerMask enemyHitBoxLayer; // �� ���̾�

    [Header("���� �˻�")]
    private bool isChange = false; // ĳ���͸� �����Ͽ����� (�� ĳ����)
    public bool isGrounded = false; // ���� ��Ҵ��� ����
    public bool canKillEnemy = false; // ���� ���� �� �ִ��� ����
    public bool isMuscle = false; // ���� ���� ����
    public bool moveOk = true; // �̵� ���� ����
    public bool isDead = false; // ��� ����
    public bool isDownKey = false; // �Ʒ�Ű �Է� ����
    [HideInInspector] public bool isRbOnRunning = false; // �߷� �ڷ�ƾ ���� ����

    [Header("������Ʈ")]
    [HideInInspector] public Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider2D; // �ڽ� �ݶ��̴�
    public RuntimeAnimatorController playerAnimatorController; // �÷��̾� �ִϸ�����
    public RuntimeAnimatorController muscleAnimatorController; // ���� ���� �ִϸ����� ��Ʈ�ѷ�
    public RuntimeAnimatorController birdAnimatorController; // �� �ִϸ����� *����

    [Header("���� ���� ũ��")]
    [SerializeField] private Vector2 groundCheckBoxSize = new Vector2(0.43f, 0.18f);
    [SerializeField] private Vector2 enemyCheckBoxSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 footPostion; // �� ��ġ
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        // �ʱ� ����
        jumpPower = minJumpPower;
        jumpBar.value = minJumpPower;
        initialScale = transform.localScale;
        initPos = transform.position;

        float x = PlayerPrefs.GetFloat("SavePointX", transform.position.x);
        float y = PlayerPrefs.GetFloat("SavePointY", transform.position.y);
        float z = PlayerPrefs.GetFloat("SavePointZ", transform.position.z);

        transform.position = new Vector3(x, y, z);

        life = PlayerPrefs.GetInt("PlayerLife", 3); // PlayerPrefs���� life ���� �ҷ����� 
        isDead = false;
    }

    void Update()
    {
        //MoveOk�� true�϶���
        if (moveOk && isChange == false)
        {
            // ������ȯ �� �̵�
            moveDir = Input.GetAxisRaw("Horizontal");

            if (moveDir != 0)
            {
                spriteRenderer.flipX = moveDir == -1;
            }
            // �ִϸ��̼�
            animator.SetBool("isWalking", moveDir != 0);

            bool isFalling = rigid.velocity.y < 0.01f;
            if (isFalling)
            {
                animator.SetBool("isJumping", !isGrounded);
            }

            #region Ű �ٿ� üũ            
            // ĳ���� ��ȯ
            if (Input.GetKeyDown(KeyCode.Tab)) // ���� ������, ���� ����(��)�� �ȵ� �����̸�
            {
                rigid.velocity = Vector3.zero;
                if (isChange == false)
                {
                    isChange = true; // ���� �Ϸ�
                    animator.runtimeAnimatorController = birdAnimatorController;
                }
                else if (isChange == true)
                {
                    isChange = false;
                    animator.runtimeAnimatorController = playerAnimatorController;
                }
            }
            // ����
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

                    // �̺�Ʈ ȣ��
                    OnPlayerJumped?.Invoke(jumpPower);
                }
                jumpPower = minJumpPower;
            }

            // ���� �Ŀ� ��¡
            if (Input.GetButton("Jump"))
            {
                if (jumpPower <= maxJumpPower)
                {
                    jumpPower += 20f * Time.deltaTime;
                }
            }

            // DownKey�� �������� ����
            isDownKey = Input.GetKey(KeyCode.DownArrow) ? true : false;

            #endregion

            if (transform.position.y < -7.5f && !(gameObject.layer == LayerMask.NameToLayer("PlayerDie")))
                OnDie();

            // ī�޶��� �����̻����� �̵����ϰ�
            // CanMovePos();

            jumpBar.value = jumpPower;
        }
        
        // ���� ĳ���Ͱ� �ٲ������
        if (moveOk && isChange == true)
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                rigid.velocity = Vector2.up * 3.5f;
            }
        }
        
    }
    void FixedUpdate()
    {
        // �ٴ� �浹 üũ
        CheckGroundAndEnemy();

        if (moveOk && isChange == false)
        {
            rigid.AddForce(new Vector2(moveDir * moveSpeed, 0));
            if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
            {
                // rigid.velocity.y�� �״�� �����ϸ鼭 x�� �ӵ��� ����
                rigid.velocity = new Vector2(Mathf.Sign(rigid.velocity.x) * maxSpeed, rigid.velocity.y);
            }

            if (rigid.velocity.y > maxJumpPower)
                rigid.velocity = new Vector2(rigid.velocity.x, maxJumpPower);

            
        }
    }

    // ī�޶��� �����̻����� �̵����ϰ�
    private void CanMovePos()
    {
        Vector3 screenPoint = playerCamera.WorldToViewportPoint(transform.position);

        if (screenPoint.x < 0f)
            screenPoint.x = 0f;

        transform.position = Camera.main.ViewportToWorldPoint(screenPoint);
    }


    // ���Ͻ�Ű��
    public IEnumerator RigidbodyOn(Collision2D cs, float time)
    {
        isRbOnRunning = true;

        yield return new WaitForSeconds(time);
        if (cs.rigidbody != null) // ���⿡���� null üũ�� �ؾ� ����
        {
            cs.rigidbody.isKinematic = false;
            SavePoint savePoint = FindObjectOfType<SavePoint>();
            if (savePoint != null)
                savePoint.SavePointRbOn();
        }

        isRbOnRunning = false;
    }

    //���� �����Ҷ�
    public void OnAttack(GameObject enemy)
    {
        //�� ���
        Enemy monster = enemy.GetComponentInParent<Enemy>();
        monster.gameObject.tag = "DeadEnemy";
        monster.OnEnemyDie();     

        //������� ���� �����Ǳ�
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * attackJumpPower, ForceMode2D.Impulse);


        //��������
        gameManager.stagePoint += 100;

    }

    //Player ���
    public void OnDie()
    {
        if (!isDead)
        {
            //Sprite �ݶ��̴� ����
            boxCollider2D.enabled = false;

            EnabledHeadSensor(false);

            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
            moveOk = false;
            gameObject.layer = LayerMask.NameToLayer("PlayerDie");
            life--;
            PlayerPrefs.SetInt("PlayerLife", life); // life ���� PlayerPrefs�� �����մϴ�.
            PlayerPrefs.Save();
            gameManager.SoundOn("death");

            StartCoroutine(DieMotion());
        }

    }
    //������ ������ó�� �װ��ϴ� ��� �ڷ�ƾ
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
        //Sprite ����
        StartCoroutine(DeActive(1));
    }

    // �÷��̾� ��Ȱ��ȭ
    IEnumerator DeActive(float time)
    {
        yield return new WaitForSeconds(time);
        //gameObject.SetActive(false);
        gameManager.DeathSceneOn(); // ������ Ű��
    }

    // ���� ����, �� üũ Ȯ��
    private void CheckGroundAndEnemy()
    {
        Bounds bounds = boxCollider2D.bounds;
        footPostion = new Vector2(bounds.center.x, bounds.min.y);
        canKillEnemy = Physics2D.OverlapBox(footPostion, enemyCheckBoxSize, 0, enemyHitBoxLayer); // �� �ؿ� ���� �Ӹ��� ������ ���� �� ����
        isGrounded = Physics2D.OverlapBox(footPostion, groundCheckBoxSize, 0, platFormLayer);

        EnabledHeadSensor(!isGrounded); // �����Ҷ��� �Ӹ� ���� Ű��
    }

    // ����� ����
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

    // ��� ���� ���
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

    // �Ӹ� ��������
    public void EnabledHeadSensor(bool enable)
    {
        // ��� �ڽ� ������Ʈ�� �߿��� 'HeadSensor' �±׸� ���� ������Ʈ�� ã��
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

    #region �����
    // ����� �׸���
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = boxCollider2D.bounds;
            Vector2 footPosition = new Vector2(bounds.center.x, bounds.min.y);
            Gizmos.DrawWireCube(footPosition, groundCheckBoxSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(footPosition, enemyCheckBoxSize);
        }
    }
#endif

#endregion
}
