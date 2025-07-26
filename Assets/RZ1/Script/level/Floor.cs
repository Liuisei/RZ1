using System;
using System.Collections.Generic;
using System.Linq;
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

    [ContextMenu("TileReset")]
    public void TileReset()
    {
        ClearTile();
        AllTileGenerate();
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
        a.transform.SetParent(_tilesParent);
    }

    [ContextMenu("WallReset")]
    public void WallReset()
    {
        ClearWall();
        AllWallGenerate();
    }
    public void AllWallGenerate()
    {
        int min = -_tileRange;
        int max = _tileRange;

        for (int i = min; i <= max; i++)
        {
            GenerateWall(i, max + 1);     // Top
            GenerateWall(i, min - 1);     // Bottom
            GenerateWall(max + 1, i);     // Right
            GenerateWall(min - 1, i);     // Left
        }
    }

    public void GenerateWall(int x, int y)
    {
        var wall = Instantiate(_wallPrefab, _wallsParent);
        wall.transform.localPosition = new Vector3(x * _tileToTileRange, 0, y * _tileToTileRange);
        wall.name = $"Wall({x},{y})";
    }

    [ContextMenu("ClearTile")]
    public void ClearTile()
    {
        var tileChildList = _tilesParent.GetComponentsInChildren<Transform>(true).Skip(1).ToArray();

        foreach (var t in tileChildList)
        {
            if (t != null)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }
    [ContextMenu("ClearWall")]
    public void ClearWall()
    {
        var wallChildList = _wallsParent.GetComponentsInChildren<Transform>(true).Skip(1).ToArray();
        foreach (var t in wallChildList)
        {
            if (t != null)
            {
                DestroyImmediate(t.gameObject);
            }
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



