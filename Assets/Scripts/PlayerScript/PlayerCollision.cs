using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerMove pm;
    public float fallWaitTime; // ���� Ŀ�� �� �ٴ��� �������� �ð�
    private void Awake()
    {
        pm = GetComponent<PlayerMove>();
    }

    #region Collision, Trigger ����    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        // Enemy�� �Ӹ��� ����������
        if (gameObject.CompareTag("Player") && obj.tag == "EnemyHitBox")
        {
            if (pm.canKillEnemy) // Enemy �Ӹ��� �� �ؿ� �ΰ������� ���� �� ����
            {
                pm.OnAttack(collision.gameObject);
            }
        }
        // ���� or Enemy�� �浹�� ���
        if (gameObject.CompareTag("Player") && obj.tag == "Enemy" || obj.CompareTag("Spike"))
        {
            pm.OnDie();
        }

        if (obj.tag == "Item")
        {
            if (!pm.isMuscle)
            {
                pm.rigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

                pm.EnabledHeadSensor(false);

                gameObject.tag = "Player_muscle";
                transform.localScale = pm.initialScale * 6f;
                // �÷��̾��� ���̵� ����
                float heightIncrease = (transform.localScale.y - pm.initialScale.y) / 2;
                transform.position = new Vector2(transform.position.x, transform.position.y + heightIncrease);
                //animator.runtimeAnimatorController = muscleAnimatorController;
                pm.isMuscle = true;
            }
            Destroy(collision.gameObject);
        }

        /*// TODO ������ �Ǹ� ������Ʈ�� �μ���
        if (gameObject.tag == "Player_muscle" && collision.gameObject.CompareTag("CanDestroy") || collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.rigidbody != null)
            {
                float randomAngle = UnityEngine.Random.Range(0f, 360f);

                collision.rigidbody.isKinematic = false;
                collision.collider.enabled = false;
                // ������Ʈ�� ������ ȸ�� ����

                collision.rigidbody.AddForce(Vector2.right * moveDir * 10f, ForceMode2D.Impulse);
                // ������Ʈ�� �������� ȸ������ ����
                float torqueAmount = moveDir * 20f; // ȸ������ ��� ���� ����
                collision.rigidbody.AddTorque(torqueAmount, ForceMode2D.Impulse);
            }
        }*/
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // �������϶� �ٴڶ������� 
        if (gameObject.tag == "Player_muscle" && collision.gameObject.layer == pm.platFormLayer)
        {
            if (collision.rigidbody != null && !pm.isRbOnRunning)
            {
                StartCoroutine(pm.RigidbodyOn(collision, fallWaitTime));
            }
        }
    }

    #endregion
}
