using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentBox : Box
{
    protected override void Awake()
    {
        base.Awake();
        boxCollider2D.enabled = false;        
    }

    protected override void Update()
    {
        base.Update();
        if (isBoxOn == true)
        {
            boxCollider2D.enabled = true;
            spriteRenderer.color = new Color(1, 1, 1, 1);
            spriteRenderer.enabled = true;
        }
    }

    private void SpriteRendererFalse()
    {
        spriteRenderer.enabled = false;
    }

    private void OnEnable()
    {
        OnInitEvent += SpriteRendererFalse;
    }

    private void OnDisable()
    {
        OnInitEvent -= SpriteRendererFalse;
    }
}
