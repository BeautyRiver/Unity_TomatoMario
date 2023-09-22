using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class BricksTile : MonoBehaviour
{
    public Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void MakeDot ( Vector3 pos)
    {
        Vector3Int cellPostion = tilemap.WorldToCell(pos);
        tilemap.SetTile(cellPostion, null);
    }
}
