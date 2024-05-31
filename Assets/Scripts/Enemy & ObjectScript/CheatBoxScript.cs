using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class CheatBoxScript : MonoBehaviour
{
    public enum boxType
    {
        Cheat,
        Spawn, // �Ӹ� �ڴ� ����
        DeathGreenHole,
        MovingPlatform,
    }
    public boxType type;

      
    private bool isMoving = false; // ���ڰ� �����̰� �ִ��� Ȯ���ϴ� �÷���
    private GameObject parentObj; // �ڽ�
    private SpriteRenderer sprParent;
    private Collider2D colParent;
    private Rigidbody2D rigParent;

    
    
    void Start()
    {
        parentObj = transform.parent.gameObject;
        sprParent = parentObj.GetComponent<SpriteRenderer>();
        colParent = parentObj.GetComponent<Collider2D>();
        rigParent = parentObj.GetComponent<Rigidbody2D>();
    }
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        //����ũ�ڽ�
        if (collision.gameObject.tag == "Player_Head")
        {
            if (type == boxType.Cheat && !isMoving)
            {
                StartCoroutine(MoveBox());
            }
            if (type == boxType.Spawn && colParent.enabled == false && sprParent.enabled == false)
            {
                sprParent.enabled = true;
                colParent.enabled = true;
            }            
            if (type == boxType.MovingPlatform)
            {
                rigParent.isKinematic = false;
                rigParent.gravityScale = 7;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // Ȧ
        if (collision.gameObject.tag == "Player")
        {           
            if (type == boxType.DeathGreenHole && PlayerData.Instance.Player.isDownKey)
            {
                PlayerData.Instance.Player.InGreenHole(transform.position);
            }
        }
    }

    // ���ڰ� ���� �ö󰡴� �Լ�
    IEnumerator MoveBox()
    {
        isMoving = true; // ������ ����
        parentObj.transform.DOMoveY(parentObj.transform.position.y + 2.5f, 0.3f); 
        isMoving = false; // ������ ����
        yield return new WaitForSeconds(0.6f); // 0.6�� ��� (���ڰ� �����̴� ���� ���)
    }
}
