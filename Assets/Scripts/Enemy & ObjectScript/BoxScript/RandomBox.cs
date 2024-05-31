using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RandomBox : Box
{
    [Header("랜덤 오브젝트")]
    public GameObject[] ranObj;
    public GameObject selectObj;
    public Sprite brokenBoxImg;
    private bool isSpawned = false;
    protected override void Awake()
    {
        base.Awake();
        if (ranObj.Length > 0)
        {
            int ranIndex = Random.Range(0, ranObj.Length); // 랜덤 인덱스
            selectObj = Instantiate(ranObj[ranIndex], transform.position, Quaternion.identity, transform); // 랜덤 오브젝트 생성
            selectObj.SetActive(false); // 랜덤 오브젝트 비활성화
        }

    }
    protected override void Update()
    {
        base.Update();
        if (isBoxOn == true && isSpawned == false)
        {
            spriteRenderer.sprite = brokenBoxImg;
            isSpawned = true;
            selectObj.SetActive(true); // 랜덤 오브젝트 활성화
            selectObj.GetComponent<DOTweenAnimation>().DOPlay();
        }
    }

    private void OnDisable()
    {
        // 플레이 모드가 종료되면 자식 오브젝트 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
