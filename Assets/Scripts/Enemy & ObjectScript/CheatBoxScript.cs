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
        Spawn, // 머리 박는 상자
        DeathGreenHole,
        MovingPlatform,
    }
    public boxType type;

      
    private bool isMoving = false; // 상자가 움직이고 있는지 확인하는 플래그
    private GameObject parentObj; // 박스
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
        //훼이크박스
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
        // 홀
        if (collision.gameObject.tag == "Player")
        {           
            if (type == boxType.DeathGreenHole && PlayerData.Instance.Player.isDownKey)
            {
                PlayerData.Instance.Player.InGreenHole(transform.position);
            }
        }
    }

    // 상자가 위로 올라가는 함수
    IEnumerator MoveBox()
    {
        isMoving = true; // 움직임 시작
        parentObj.transform.DOMoveY(parentObj.transform.position.y + 2.5f, 0.3f); 
        isMoving = false; // 움직임 종료
        yield return new WaitForSeconds(0.6f); // 0.6초 대기 (상자가 움직이는 동안 대기)
    }
}
