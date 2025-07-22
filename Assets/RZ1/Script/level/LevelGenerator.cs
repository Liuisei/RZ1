using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Floor _srartRoomPrefab;
    [SerializeField] private Floor[] _floorPrefabs;
    [SerializeField] private Floor _goalPrefab;
    [SerializeField] private int _floorToFloorRange = 20;
    [SerializeField] private int _floorCountToGoal = 10;
    [SerializeField] private NavMeshSurface navMeshSurface;
    private Vector2Int _startPos;
    private List<Vector2Int> _startToEndList = new();
    private Vector2Int _endPos;

    private Dictionary<(int x, int y), Floor> _floorMap;

    private void Start()
    {
        _floorMap = new Dictionary<(int x, int y), Floor>();
        GanerateFloor().Forget();

    }
    private async UniTask GanerateFloor()
    {
        GenerateStartFloor();
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        await GenerateStartToGoalPos();
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        navMeshSurface.BuildNavMesh();
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        await navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

    }
    void GenerateStartFloor()
    {
        GenerateGuild(0, 0, _srartRoomPrefab);
        _startPos = new Vector2Int(0, 0);
    }
    private async UniTask GenerateStartToGoalPos()
    {
        Debug.Log("Generating path to goal (with backtracking)...");

        Stack<Vector2Int> _pathStack = new Stack<Vector2Int>();
        _pathStack.Push(_startPos);

        int createdFloorCount = 0;

        while (createdFloorCount < _floorCountToGoal && _pathStack.Count > 0)
        {
            Vector2Int current = _pathStack.Peek();
            List<(Vector2Int, Vector2Int)> candidates = GenerateCheck(current, 1); // 範囲付き探索

            if (candidates.Count > 0)
            {
                Floor floor = _floorPrefabs[Random.Range(0, _floorPrefabs.Length)];
                //Floor floor = _floorPrefabs[0];
                (Vector2Int, Vector2Int) selectedCandidate = candidates[Random.Range(0, candidates.Count)];
                Vector2Int next = selectedCandidate.Item1;
                _floorMap[(current.x, current.y)].OpenDoor(-selectedCandidate.Item2);
                GenerateGuild(next.x, next.y, floor, selectedCandidate.Item2);
                _pathStack.Push(next);
                createdFloorCount++;
            }
            else
            {
                _pathStack.Pop(); // Dead end
            }

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (_pathStack.Count > 0)
        {
            Vector2Int goalPosPreview = _pathStack.Peek();
            List<(Vector2Int, Vector2Int)> candidates = GenerateCheck(goalPosPreview, 1); // 範囲付き探索
            (Vector2Int, Vector2Int) selectedCandidate = candidates[Random.Range(0, candidates.Count)];
            Vector2Int next = selectedCandidate.Item1;
            _floorMap[(goalPosPreview.x, goalPosPreview.y)].OpenDoor(-selectedCandidate.Item2);
            GenerateGuild(next.x, next.y, _goalPrefab, selectedCandidate.Item2);
        }
        else
        {
            Debug.LogWarning("Goal not placed: No valid path found.");
        }
    }

    private void GenerateGuild(int x, int y, Floor floor, Vector2Int openDoorDir = default)
    {
        Floor newFloor = Instantiate(floor, new Vector3(x * _floorToFloorRange, 0, y * _floorToFloorRange), Quaternion.identity);
        newFloor.transform.SetParent(transform);
        newFloor.OpenDoor(openDoorDir);
        _floorMap[(x, y)] = newFloor;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="range"></param>
    /// <returns>List<(Vector2Int,Vector2Int)> (pos,directions) pair</returns>
    private List<(Vector2Int, Vector2Int)> GenerateCheck(Vector2Int origin, int range = 1)
    {
        List<Vector2Int> directions = new()
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };


        List<(Vector2Int, Vector2Int)> result = new();

        foreach (var dir in directions)
        {
            Vector2Int center = origin + dir * range;
            int wing = range - 1;
            int squareSize = wing * 2 + 1;
            Vector2Int squareTopLeft = center + new Vector2Int(-wing, wing);

            bool hasFloor = false;

            for (int y = 0; y < squareSize && !hasFloor; y++)
            {
                for (int x = 0; x < squareSize && !hasFloor; x++)
                {
                    Vector2Int pos = squareTopLeft + new Vector2Int(x, -y);
                    if (_floorMap.ContainsKey((pos.x, pos.y)))
                    {
                        hasFloor = true;
                        goto SkipThisDirection; // 内側2重ループを即抜ける
                    }
                }
            }

        SkipThisDirection:
            if (!hasFloor)
            {
                result.Add((center, dir));
            }
        }
        return result;
    }
}
public enum GuildType
{
    None = 0,
    Start = 1,
    Goal = 2,
    Floor = 3,
    HallwayHor = 4,
    HallwayVar = 5,
}
