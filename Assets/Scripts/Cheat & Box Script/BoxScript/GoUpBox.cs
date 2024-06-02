using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GoUpBox : Box
{
    private DOTweenAnimation doTweenAnimation;
    private bool isFirstStart = true;
    protected override void Awake()
    {
        base.Awake();        
        doTweenAnimation = GetComponent<DOTweenAnimation>();
    }
    
    protected override void Update()
    {
        base.Update();
        if (isBoxOn == true)
        {
            doTweenAnimation.DOPlay();            
        }        
    }

}