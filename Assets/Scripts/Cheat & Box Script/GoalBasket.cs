using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalBasket : MonoBehaviour
{
    public GameObject clearScene;

    private Image clearImage;
    private TextMeshProUGUI clearText;
    public PlayerMove playerMove;
    private void Start()
    {
        clearScene = GameObject.FindWithTag("ClearScene"); 
        playerMove = GameManager.instance.player;
        clearImage = clearScene.GetComponentInChildren<Image>();
        clearText = clearScene.GetComponentInChildren<TextMeshProUGUI>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (MakerManager.instance.isGameMaker)
            return;

        if (collision.CompareTag("Player"))
        {
            // FindObjectOfType<GameManager>().SoundOn("clear");
            playerMove.Stop();
            playerMove.gameObject.layer = LayerMask.NameToLayer("PlayerDie");                        
            playerMove.isDead = true;
            clearImage.DOFade(1, 1.0f).OnComplete(() =>
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
        SceneManager.LoadScene("Title Scene");
    }
}
