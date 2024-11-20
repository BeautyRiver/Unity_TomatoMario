using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
public class MouseSelector : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase changeTile;
    public TileBase selectTile;
    
    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePos);
        transform.position = tilemap.CellToWorld(cellPosition) + tilemap.cellSize / 2;

        if (Input.GetMouseButton(0))
        {
            tilemap.SetTile(cellPosition, changeTile);
        }
    }
}
