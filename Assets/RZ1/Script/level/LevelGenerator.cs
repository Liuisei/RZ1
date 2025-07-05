using NUnit.Framework.Internal;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject _firstRoomPrefab;
    [SerializeField] private GameObject[] _floorPrefabs;
    [SerializeField] private GameObject _goalPrefab;
    [SerializeField] private int _rangeHorizontal = 10;
    [SerializeField] private int _rangeVertical = 10;
    [SerializeField] private int _floorToFloorRange = 20;
    [SerializeField] private int _minimumFloorCount = 5;

    private int[][] _floorMap;

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void GenerateHomeRoom()
    {
        // Logic to generate the first room of the level
        Debug.Log("Generating first room...");
        Instantiate(_firstRoomPrefab, Vector3.zero, Quaternion.identity);
        // Add your room generation code here
    }

    void GenerateFloor()
    {

    }

    void GenerateGoal()
    {
        Debug.Log("Generating goal...");
    }
}
