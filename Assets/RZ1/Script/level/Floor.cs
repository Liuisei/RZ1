using System;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private string _floorName;
    [SerializeField] private Transform _tilesParent;
    [SerializeField] private Transform _wallsParent;
    [Range(0, 100)][SerializeField] private int _tileRange;
    [Range(0, 100)][SerializeField] private int _hallwayRange;
    private List<Door> _openDoors;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GulidCellMeshData[] _wallMeshs;
    [SerializeField] private GulidCellMeshData[] _tileMeshs;
    [SerializeField] private int _tileToTileRange = 5;

    private Dictionary<(int x, int y), GameObject> _gulitTile;

    private int _tileSideLangh => _tileRange * 2 + 1;

    public GuildType GuildType;

    [ContextMenu("Generate")]
    public void GenerateFloorTile()
    {
        Clear();
        _gulitTile = new Dictionary<(int x, int y), GameObject>();
        AllTileGenerate();
        GenerateWall(0, 0);
    }
    public void AllTileGenerate()
    {
        for (int x = -_tileRange; x <= _tileRange; x++)
        {
            for (int y = -_tileRange; y <= _tileRange; y++)
            {
                GenerateTile(x, y);
            }
        }
    }
    public void GenerateTile(int x, int y)
    {
        var a = Instantiate(_tilePrefab, transform);
        a.transform.localPosition = new Vector3(x * _tileToTileRange, 0, y * _tileToTileRange);
        a.name = $"Tile({x},{y})";
        _gulitTile[(x, y)] = a;
    }
    public void GenerateWall(int x,int y)
    {
        var a = Instantiate(_wallPrefab, transform);
        a.transform.localPosition = new Vector3(x * _tileToTileRange, 0, y * _tileToTileRange);
        a.name = $"wall({x},{y})";
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (_gulitTile == null) return;
        Debug.Log(_gulitTile.Count);
        foreach (var tile in _gulitTile)
        {
            DestroyImmediate(tile.Value);
        }

    }

    /// <summary>
    /// 上から時計回り1234
    /// </summary>
    /// <param name="floorIndex"></param>
    public void OpenDoor(Vector2Int openDoorDir = new Vector2Int())
    {
        int floorIndex = 0;
        if (openDoorDir == Vector2Int.up)
        {
            floorIndex = 1;
        }
        else if (openDoorDir == Vector2Int.right)
        {
            floorIndex = 2;
        }
        else if (openDoorDir == Vector2Int.down)
        {
            floorIndex = 3;
        }
        else if (openDoorDir == Vector2Int.left)
        {
            floorIndex = 4;
        }
        else if (openDoorDir == default)
        {
            floorIndex = 0;
        }

        if (floorIndex > 0)
        {
            Door door = _openDoors[(floorIndex) % 4];
            door.HideDoor.gameObject.SetActive(false);
            if (door.Showhallway != null)
            {
                door.Showhallway.gameObject.SetActive(true);
            }
        }
    }

    [Serializable]
    private class Door
    {
        public Transform HideDoor;
        public Transform Showhallway;
    }

    [Serializable]
    private class GulidCellMeshData
    {
        public Mesh Mesh;
        public int Probability;
    }
}



