using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerManager : MonoBehaviour
{
    public static MakerManager instance;
    public bool isGameMaker;
    public Text infoText;
    public float moveSpeed = 5f; // ī�޶� �̵� �ӵ�
    public float smoothTime = 0.3f; // �ε巴�� �̵��ϴ� �ð�
    private Vector3 velocity = Vector3.zero; // SmoothDamp���� ���Ǵ� �ӵ� ���尪

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        ToggleGameStateOnKeyPress();
        if (!isGameMaker)
            return;

        CameraMoveControll();
    }

    private void ToggleGameStateOnKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            GameState();
    }

    private void CameraMoveControll()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // ���� ī�޶� ��ġ
        Vector3 currentPosition = Camera.main.transform.position;

        // ��ǥ ��ġ ���
        Vector3 targetPosition = currentPosition + new Vector3(x, y, 0) * moveSpeed * Time.deltaTime;

        // SmoothDamp�� ����Ͽ� �ε巴�� �̵�
        Camera.main.transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, smoothTime);
    }

    public void GameState()
    {
        isGameMaker = !isGameMaker;
        if (isGameMaker)
        {
            infoText.text = "F5�� ������ ������ �÷��� �غ� �� �־��!";
            
        }
        else
        {
            infoText.text = "F5�� ������ �ٽ� ������ ���� �� �־��!";
            PlayerPrefs.DeleteAll();
            GameManager.instance.player.InitPlayer();
        }
    }

}
