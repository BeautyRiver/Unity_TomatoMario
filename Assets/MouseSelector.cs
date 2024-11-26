using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseSelector : MonoBehaviour
{
    [Header("# 아이템 관리")]
    public Tilemap tilemap;
    public TileBase changeTile;
    public GameObject[] gameObjects; // 배치할 오브젝트들

    private GameObject tempObj; // 미리보기 오브젝트
    private bool isPlacingObject = false; // 오브젝트 배치 중인지 확인
    private Vector3 mousePos; // 마우스 월드 좌표
    private Vector3Int cellPosition; // 타일맵 셀 좌표

    void Update()
    {
        if (!MakerManager.instance.isGameMaker) // 게임 메이커 모드가 아니면 동작 안함
            return;

        if (GameManager.instance.isPaused) // 일시정지 상태면 동작 안함
            return;

        // 마우스 위치 계산
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cellPosition = tilemap.WorldToCell(mousePos);

        if (MakerManager.instance.itemIndex == 1) // 타일 설치
        {
            TileSet();
        }
        else // 오브젝트 배치
        {
            ObjectSet();
        }
    }

    private void TileSet()
    {
        transform.position = tilemap.CellToWorld(cellPosition) + tilemap.cellSize / 2;

        if (Input.GetMouseButton(0)) // 왼쪽 클릭 중
        {
            tilemap.SetTile(cellPosition, changeTile);
        }
    }

    private void ObjectSet()
    {
        // 마우스 위치 업데이트
        transform.position = new Vector2(mousePos.x, mousePos.y);
        if (Input.GetMouseButtonDown(0)) // 클릭 시작
        {
            isPlacingObject = true;

            // 미리보기 오브젝트 생성
            if (tempObj == null)
            {
                tempObj = Instantiate(gameObjects[MakerManager.instance.itemIndex - 2], transform.position, Quaternion.identity);
                tempObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // 반투명 처리
            }
        }

        if (isPlacingObject && tempObj != null) // 드래그 중
        {
            tempObj.transform.position = transform.position; // 미리보기 위치 업데이트
        }

        if (Input.GetMouseButtonUp(0) && isPlacingObject) // 클릭 종료
        {
            isPlacingObject = false;

            // 최종 위치에 오브젝트 배치
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        if (tempObj != null)
        {
            tempObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f); // 원래 색상 복구
            tempObj = null; // 미리보기 오브젝트 초기화
        }
    }
}
