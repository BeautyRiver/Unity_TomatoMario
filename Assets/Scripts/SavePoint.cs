using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Sprite checkPointSaveNo;
    public Sprite checkPointSaveOk;
    private SpriteRenderer spr;
    private int isSavePointChecked;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        isSavePointChecked = PlayerPrefs.GetInt("SavePointCheck", 0);
        if(isSavePointChecked == 0 )
            spr.sprite = checkPointSaveNo;
        else
            spr.sprite = checkPointSaveOk;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isSavePointChecked == 0)
        {
            spr.sprite = checkPointSaveOk;
            isSavePointChecked = 1;
            Vector3 savePoint = collision.transform.position;
            PlayerPrefs.SetInt("SavePointCheck", 1);
            PlayerPrefs.SetFloat("SavePointX", savePoint.x);
            PlayerPrefs.SetFloat("SavePointY", savePoint.y);
            PlayerPrefs.SetFloat("SavePointZ", savePoint.z);
            PlayerPrefs.Save();
        }
    }

    public void SavePointRbOn()
    {
        rb.isKinematic = false;
    }
}
