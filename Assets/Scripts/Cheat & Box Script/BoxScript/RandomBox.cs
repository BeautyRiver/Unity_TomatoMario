using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
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
            selectObj = ranObj[ranIndex].gameObject; // 랜덤 오브젝트 선택
            selectObj.transform.position = transform.position;
            selectObj.SetActive(false); // 랜덤 오브젝트 비활성화
        }
    }

    protected override void Update()
    {
        base.Update();
        if (isBoxOn == true && isSpawned == false)
        {            
            isSpawned = true;
            spriteRenderer.sprite = brokenBoxImg;
            selectObj.SetActive(true); // 랜덤 오브젝트 활성화
            selectObj.GetComponent<DOTweenAnimation>().DOPlay();
        }
    }  
}
