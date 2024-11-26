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
    public GameObject batchMouse; // 배치할 마우스
    public GameObject spriteChanger; // 타일 변경 버튼
    public Transform selector; // 선택된 아이템을 표시할 이미지



    public float moveSpeed = 5f; // 카메라 이동 속도
    public float smoothTime = 0.3f; // 부드럽게 이동하는 시간
    private Vector3 velocity = Vector3.zero; // SmoothDamp에서 사용되는 속도 저장값
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        ToggleGameStateOnKeyPress(); // 게임 상태 전환
        if (!isGameMaker)
            return;
        UpdateItemIndex(); // 아이템 인덱스 업데이트
        CameraMoveControll(); // 카메라 이동
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
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) // 숫자 키 입력 확인
            {
                itemIndex = i;
                selector.position = itemList.transform.GetChild(i - 1).position;
                break; // 숫자 입력이 처리되면 반복문 종료
            }
        }
    }
    private void CameraMoveControll()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        // 현재 카메라 위치
        Vector3 currentPosition = Camera.main.transform.position;

        // 목표 위치 계산
        Vector3 targetPosition = currentPosition + new Vector3(x, y, 0) * moveSpeed * Time.fixedDeltaTime;

        // SmoothDamp를 사용하여 부드럽게 이동
        Camera.main.transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, smoothTime);
    }

    public void GameState()
    {
        isGameMaker = !isGameMaker;
        if (isGameMaker)
        {
            infoText.text = "F5를 누르면 [게임을 플레이] 해볼 수 있어요!";
            itemList.SetActive(true);
            GameManager.instance.player.MakerMode();
            batchMouse.SetActive(true);
            spriteChanger.SetActive(true);
        }
        else
        {
            infoText.text = "F5를 누르면 [게임을 제작] 할 수 있어요!";
            itemList.SetActive(false);
            PlayerPrefs.DeleteAll();
            GameManager.instance.player.InitPlayer();
            batchMouse.SetActive(false);
            spriteChanger.SetActive(false);
        }
    }

}
