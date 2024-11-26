using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEnemyTrigger : DetectOption
{
    private void Awake() 
    {
        adaptPos.x = transform.position.x;
    }
    protected override void Update()
    {
        base.Update();
        // 위가 감지되면
        if (isDetectUp)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isDetectUp = false; // 감지 초기화
        }
        // 아래가 감지되면
        else if (isDetectDown)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isDetectDown = false; // 감지 초기화
        }
    }
}
