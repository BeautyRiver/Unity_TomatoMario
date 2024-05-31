using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public enum Type
    {
        Spider_Ai,
        Spider_NoAi,
        Bee,
    }
    public Type type;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider;
    public int nextMove;
    bool isAlive;
    public int dirBee = -1;
    public int dirSpider = 1;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        isAlive = true;

        if (type == Type.Spider_Ai)
            Think();
    }

    void FixedUpdate()
    {
        if (type == Type.Spider_Ai)
        {
            //Platform Check (���������� �������� �ʴ� ����)
            if (isAlive)
            {
                Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y); // <<�� �̵����̶�� x + -1�� �Ͽ� ����üũ
                Debug.DrawRay(frontVec, Vector2.down, new Color(1, 0, 0));
                RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 2, LayerMask.GetMask("Platform"));

                if (rayHit.collider == null)
                {
                    Turn();
                }

                //Move    
                rigid.velocity = new Vector2(nextMove * 1.5f, rigid.velocity.y);
            }
        }
        else if (type == Type.Spider_NoAi)
        {
            if (!IsGrounded(dirSpider) || IsWalled(dirSpider))
            {
                dirSpider *= -1;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            transform.Translate(Vector2.left * -dirSpider * 2.0f * Time.deltaTime);
            anim.SetInteger("WalkSpeed", 1);
        }

        else if (type == Type.Bee)
        {
            // ���߿� ���� �ʰ�, ���� �ε����ų� �ٴ��� ���� ���� ���� ��ȯ
            if (IsJumped(dirBee) && (!IsGrounded(dirBee) || IsWalled(dirBee)))
            {
                dirBee *= -1;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            
            transform.Translate(Vector2.right * dirBee * 2.0f * Time.deltaTime);
            anim.SetInteger("WalkSpeed", 1);            
        }
    }
    void OnEnable()
    {
        PlayerMove.OnPlayerJumped += HandlePlayerJump;
    }

    void OnDisable()
    {
        PlayerMove.OnPlayerJumped -= HandlePlayerJump;
    }

    void HandlePlayerJump(float jumpPower)
    {
        if (type == Type.Bee && IsGrounded(dirBee))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded(int dir)
    {
        Vector2 downVec = new Vector2(rigid.position.x + dir * 0.5f, rigid.position.y);

        // �Ʒ��� ����ĳ��Ʈ
        RaycastHit2D rayHitDown = Physics2D.Raycast(downVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        Debug.DrawRay(downVec, Vector2.down, new Color(1, 0, 0));

        // ���̳� �ٴ��� �����ϸ� true ��ȯ
        return (rayHitDown.collider != null);
    }

    private bool IsJumped(int dir)
    {
        Vector2 downVec = new Vector2(rigid.position.x, rigid.position.y);

        // �Ʒ��� ����ĳ��Ʈ
        RaycastHit2D rayHitDown = Physics2D.Raycast(downVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        Debug.DrawRay(downVec, Vector2.down, new Color(0, 0, 1));

        // �ٴ��� �����ϸ� true ��ȯ
        return (rayHitDown.collider != null);
    }

    private bool IsWalled(int dir)
    {
        Vector2 sideVec = new Vector2(rigid.position.x, rigid.position.y);
        RaycastHit2D rayHitSide = Physics2D.Raycast(sideVec, dir == 1 ? Vector2.right : Vector2.left, 1, LayerMask.GetMask("Platform"));
        Debug.DrawRay(sideVec, dir == 1 ? Vector2.right : Vector2.left, new Color(0, 1, 0));

        // ���̳� �ٴ��� �����ϸ� true ��ȯ
        return (rayHitSide.collider != null);
    }


    //����Լ� Enemy �������
    void Think()
    {
        nextMove = Random.Range(-1, 2); // -1 ~ 1 ������ ���� �� / -1:���� 0:���� 1:������

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Spritexz
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == -1;

        //Recursive (����Լ�)
        float nextThinkTime = Random.Range(1.5f, 4f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        //Flip Sprite x
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == -1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnEnemyDamaged()
    {
        isAlive = false;
        /*//Sprite �ݶ��̴� ����
        boxCollider.enabled = false;*/

        //Sprite ������
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite ����
        spriteRenderer.flipY = true;
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //Sprite ����
        Destroy(gameObject, 4f);
    }

}
