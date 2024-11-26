using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakerManager : MonoBehaviour
{
    public static MakerManager instance;
    public bool isGameMaker;
    public bool isTileSet = true;
    public int itemIndex = 1;
    public Text infoText;

    public GameObject itemList;
    public GameObject batchMouse; // ��ġ�� ���콺
    public GameObject spriteChanger; // Ÿ�� ���� ��ư
    public Transform selector; // ���õ� �������� ǥ���� �̹���



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
        ToggleGameStateOnKeyPress(); // ���� ���� ��ȯ
        if (!isGameMaker)
            return;
        UpdateItemIndex(); // ������ �ε��� ������Ʈ
        CameraMoveControll(); // ī�޶� �̵�
    }

    private void ToggleGameStateOnKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            GameState();
        if (Input.GetKeyDown(KeyCode.F6))
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        
    }

    private void UpdateItemIndex()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) // ���� Ű �Է� Ȯ��
            {
                itemIndex = i;
                selector.position = itemList.transform.GetChild(i - 1).position;
                break; // ���� �Է��� ó���Ǹ� �ݺ��� ����
            }
        }
    }
    private void CameraMoveControll()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // ���� ī�޶� ��ġ
        Vector3 currentPosition = Camera.main.transform.position;

        // ��ǥ ��ġ ���
        Vector3 targetPosition = currentPosition + new Vector3(x, y, 0) * moveSpeed * Time.fixedDeltaTime;

        // SmoothDamp�� ����Ͽ� �ε巴�� �̵�
        Camera.main.transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, smoothTime);
    }

    public void GameState()
    {
        isGameMaker = !isGameMaker;
        if (isGameMaker)
        {
            infoText.text = "F5�� ������ [������ �÷���] �غ� �� �־��!";
            itemList.SetActive(true);
            GameManager.instance.player.MakerMode();
            batchMouse.SetActive(true);
            spriteChanger.SetActive(true);
        }
        else
        {
            infoText.text = "F5�� ������ [������ ����] �� �� �־��!";
            itemList.SetActive(false);
            PlayerPrefs.DeleteAll();
            GameManager.instance.player.InitPlayer();
            batchMouse.SetActive(false);
            spriteChanger.SetActive(false);
        }
    }

}
