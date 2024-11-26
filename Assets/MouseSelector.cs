using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseSelector : MonoBehaviour
{
    [Header("# ������ ����")]
    public Tilemap tilemap;
    public TileBase changeTile;
    public GameObject[] gameObjects; // ��ġ�� ������Ʈ��

    private GameObject tempObj; // �̸����� ������Ʈ
    private bool isPlacingObject = false; // ������Ʈ ��ġ ������ Ȯ��
    private Vector3 mousePos; // ���콺 ���� ��ǥ
    private Vector3Int cellPosition; // Ÿ�ϸ� �� ��ǥ

    void Update()
    {
        if (!MakerManager.instance.isGameMaker) // ���� ����Ŀ ��尡 �ƴϸ� ���� ����
            return;

        if (GameManager.instance.isPaused) // �Ͻ����� ���¸� ���� ����
            return;

        // ���콺 ��ġ ���
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cellPosition = tilemap.WorldToCell(mousePos);

        if (MakerManager.instance.itemIndex == 1) // Ÿ�� ��ġ
        {
            TileSet();
        }
        else // ������Ʈ ��ġ
        {
            ObjectSet();
        }
    }

    private void TileSet()
    {
        transform.position = tilemap.CellToWorld(cellPosition) + tilemap.cellSize / 2;

        if (Input.GetMouseButton(0)) // ���� Ŭ�� ��
        {
            tilemap.SetTile(cellPosition, changeTile);
        }
    }

    private void ObjectSet()
    {
        // ���콺 ��ġ ������Ʈ
        transform.position = new Vector2(mousePos.x, mousePos.y);
        if (Input.GetMouseButtonDown(0)) // Ŭ�� ����
        {
            isPlacingObject = true;

            // �̸����� ������Ʈ ����
            if (tempObj == null)
            {
                tempObj = Instantiate(gameObjects[MakerManager.instance.itemIndex - 2], transform.position, Quaternion.identity);
                tempObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f); // ������ ó��
            }
        }

        if (isPlacingObject && tempObj != null) // �巡�� ��
        {
            tempObj.transform.position = transform.position; // �̸����� ��ġ ������Ʈ
        }

        if (Input.GetMouseButtonUp(0) && isPlacingObject) // Ŭ�� ����
        {
            isPlacingObject = false;

            // ���� ��ġ�� ������Ʈ ��ġ
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        if (tempObj != null)
        {
            tempObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f); // ���� ���� ����
            tempObj = null; // �̸����� ������Ʈ �ʱ�ȭ
        }
    }
}
