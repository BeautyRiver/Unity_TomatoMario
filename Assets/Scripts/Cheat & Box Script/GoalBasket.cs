using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalBasket : MonoBehaviour
{
    public Image clearScene;
    public TextMeshProUGUI clearText;
    private PlayerMove playerMove;
    private void Awake()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            FindObjectOfType<GameManager>().SoundOn("clear");
            playerMove.gameObject.layer = LayerMask.NameToLayer("PlayerDie");
            playerMove.isDead = true;
            playerMove.moveOk = false;
            clearScene.DOFade(1, 1.0f).OnComplete(() =>
            {
                clearText.DOFade(1, 1.0f).OnComplete(() =>
                {
                    StartCoroutine(goToTitle());
                });                    
            });            
        }
    }
    IEnumerator goToTitle()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Title");
    }
}
