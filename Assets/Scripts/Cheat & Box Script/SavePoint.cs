using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private bool isSaved = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (!other.CompareTag("Player") || isSaved == true)
            return;
        
        PlayerPrefs.SetFloat("SavePointX", transform.position.x);
        PlayerPrefs.SetFloat("SavePointY", transform.position.y);
        PlayerPrefs.SetFloat("SavePointZ", transform.position.z);
        animator.SetTrigger("doSave");
        isSaved = true;
    }
}
