using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite attackSprite;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }
    
}
