using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject stageImage;
    [SerializeField] private GameObject optionScreen;

    [SerializeField] private List<RectTransform> stageImageRects;
    [SerializeField] private Image[] images;
    [SerializeField] private int idx = 0;
    [SerializeField] private Vector2[] stageImageScale;
    [SerializeField] private Vector2[] stageImagePos;
    [SerializeField] private Color noneSelectColor;

    private void Awake()
    {
        // 부모 rect는 필터링        
        images = stageImage.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            stageImageRects.Add(images[i].GetComponent<RectTransform>());
        }
    }
     
    // 다음 버튼
    public void PressNextButton()
    {
        if (idx >= stageImageRects.Count - 1)
            return;

        // 맨 앞에 이미지 왼편으로 치워 버리기
        if (idx > 0)
            MoveStageImage(idx - 1, stageImagePos[0], stageImageScale[0], noneSelectColor);

        // 현재 가운데 이미지 왼쪽으로 한칸 이동
        MoveStageImage(idx, stageImagePos[1], stageImageScale[1], noneSelectColor);

        // 다음 이미지 가운데로 이동
        MoveStageImage(idx + 1, stageImagePos[2], stageImageScale[2], Color.white);
        
        if (idx + 2 <= stageImageRects.Count - 1)
            MoveStageImage(idx + 2, stageImagePos[3], stageImageScale[1], noneSelectColor);

        // 인덱스 증가
        idx++;
    }

    // 이전 버튼
    public void PressPrevButton()
    {
        if (idx <= 0)
            return;

        if (idx - 2 >= 0)
            MoveStageImage(idx - 2, stageImagePos[1], stageImageScale[1], noneSelectColor);

        MoveStageImage(idx - 1, stageImagePos[2], stageImageScale[2], Color.white);
        MoveStageImage(idx, stageImagePos[3], stageImageScale[1], noneSelectColor);

        if (idx < stageImageRects.Count - 1)
            MoveStageImage(idx + 1, stageImagePos[4], stageImageScale[0], noneSelectColor);

        // 인덱스 감소
        idx--;
    }
    

    private void MoveStageImage(int index, Vector3 pos, Vector3 scale, Color color)
    {
        if (index < 0 || index >= stageImageRects.Count) return; // 범위 체크
        stageImageRects[index].DOAnchorPos(pos, 0.5f);
        stageImageRects[index].DOScale(scale, 0.5f);
        images[index].DOColor(color, 0.5f);
    }

    public void GameStart()
    {
        SceneManager.LoadScene(idx + 1);
        PlayerPrefs.DeleteAll();
    }

    // 게임 종료
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    
   /* public void SelectCharacter(PlayerData playerData)
    {
        DataManager.instance.currentPlayerData = playerData;
        SceneManager.LoadScene(idx + 1);
    }*/

}
