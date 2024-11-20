using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public bool isOneOff = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (isOneOff || !other.CompareTag("Player"))
            return;

        // 자식 개수 확인
        int childCount = gameObject.transform.childCount;

        if (childCount > 0)
        {
            // 모든 자식들을 순회
            for (int i = 0; i < childCount; i++)
            {
                // i번째 자식 객체 가져오기
                GameObject child = gameObject.transform.GetChild(i).gameObject;

                // 자식 객체의 컴포넌트 활성화
                child.SetActive(true);
            }
        }
        
        isOneOff = true;
    }
}
