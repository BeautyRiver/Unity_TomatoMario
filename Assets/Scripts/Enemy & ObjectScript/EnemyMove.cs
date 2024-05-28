using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : Enemy
{
    public enum Type
    {
        Spider_Ai,
        Spider_NoAi,
        Bee,
    }
    public Type type;
    public int nextMove;
    bool isAlive;
    public int dirBee = -1;
    public int dirSpider = 1;

    void Awake()
    {
        base.Awake();
        isAlive = true;

        if (type == Type.Spider_Ai)
            Think();
    }

    void FixedUpdate()
    {
        if (type == Type.Spider_Ai)
        {
            //Platform Check (낭떠러지에 떨어지지 않는 로직)
            if (isAlive)
            {
                Vector2 frontVec = new Vector2(rb.position.x + nextMove * 0.5f, rb.position.y); // <<로 이동중이라면 x + -1을 하여 왼쪽체크
                Debug.DrawRay(frontVec, Vector2.down, new Color(1, 0, 0));
                RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 2, LayerMask.GetMask("Platform"));

                if (rayHit.collider == null)
                {
                    Turn();
                }

                //Move    
                rb.velocity = new Vector2(nextMove * 1.5f, rb.velocity.y);
            }
        }
        else if (type == Type.Spider_NoAi)
        {
            // if (!IsGrounded(dirSpider) || IsWalled(dirSpider))
            // {
            //     dirSpider *= -1;
            //     spriteRenderer.flipX = !spriteRenderer.flipX;
            // }

            transform.Translate(Vector2.left  * 2.0f * Time.deltaTime);
            anim.SetInteger("WalkSpeed", 1);
        }

        else if (type == Type.Bee)
        {
            // 공중에 있지 않고, 벽에 부딪혔거나 바닥이 없을 때만 방향 전환
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
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded(int dir)
    {
        Vector2 downVec = new Vector2(rb.position.x + dir * 0.5f, rb.position.y);

        // 아래쪽 레이캐스트
        RaycastHit2D rayHitDown = Physics2D.Raycast(downVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        Debug.DrawRay(downVec, Vector2.down, new Color(1, 0, 0));

        // 벽이나 바닥을 감지하면 true 반환
        return (rayHitDown.collider != null);
    }

    private bool IsJumped(int dir)
    {
        Vector2 downVec = new Vector2(rb.position.x, rb.position.y);

        // 아래쪽 레이캐스트
        RaycastHit2D rayHitDown = Physics2D.Raycast(downVec, Vector2.down, 1, LayerMask.GetMask("Platform"));

        Debug.DrawRay(downVec, Vector2.down, new Color(0, 0, 1));

        // 바닥을 감지하면 true 반환
        return (rayHitDown.collider != null);
    }

    private bool IsWalled(int dir)
    {
        Vector2 sideVec = new Vector2(rb.position.x, rb.position.y);
        RaycastHit2D rayHitSide = Physics2D.Raycast(sideVec, dir == 1 ? Vector2.right : Vector2.left, 1, LayerMask.GetMask("Platform"));
        Debug.DrawRay(sideVec, dir == 1 ? Vector2.right : Vector2.left, new Color(0, 1, 0));

        // 벽이나 바닥을 감지하면 true 반환
        return (rayHitSide.collider != null);
    }


    //재귀함수 Enemy 방향번경
    void Think()
    {
        nextMove = Random.Range(-1, 2); // -1 ~ 1 까지의 랜덤 수 / -1:왼쪽 0:멈춤 1:오른쪽

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Spritexz
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == -1;

        //Recursive (재귀함수)
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
        /*//Sprite 콜라이더 끄기
        boxCollider.enabled = false;*/

        //Sprite 데미지
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite 반전
        spriteRenderer.flipY = true;
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //Sprite 삭제
        Destroy(gameObject, 4f);
    }

}
