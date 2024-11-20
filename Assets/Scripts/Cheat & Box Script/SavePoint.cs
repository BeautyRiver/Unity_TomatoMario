using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Sprite changeSprite;
    bool isSaved = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (!other.CompareTag("Player") || isSaved == true)
            return;
        
        PlayerPrefs.SetFloat("SavePointX", transform.position.x);
        PlayerPrefs.SetFloat("SavePointY", transform.position.y);
        PlayerPrefs.SetFloat("SavePointZ", transform.position.z);

        gameObject.GetComponentInParent<SpriteRenderer>().sprite = changeSprite;
        isSaved = true;
    }
}
