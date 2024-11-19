using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int player_health;
    public Image deathScene;
    public PlayerMove player;
    public TextMeshProUGUI lifeText;

    public AudioSource jumpSource;
    public AudioSource deathSource;
    public AudioSource clearSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.OnDie();
        }
        else if(!collision.CompareTag("Platform"))
        {
            collision.gameObject.SetActive(false);
        }
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void DeathSceneOn()
    {
        deathScene.gameObject.SetActive(true);
        lifeText.text = $"x {player.life}";
        
        StartCoroutine(DeathSceneOff(2f));
    }

    IEnumerator DeathSceneOff(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        deathScene.gameObject.SetActive(false);        
    }

    // PlayerPrefs으로 저장된 Life초기화
    public void LifeInitBtn()
    {        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        player.life = PlayerPrefs.GetInt("PlayerLife", 3); // PlayerPrefs에서 life 값을 불러오기 
        SceneManager.LoadScene(0);
    }

    public void SoundOn(string action)
    {
        switch (action)
        {
            case "jump":
                jumpSource.Play();
                break;
            case "death":
                deathSource.Play();
                break;
            case "clear":
                clearSource.Play();
                break;
        }
    }
}
