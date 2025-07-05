using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct FloorData
{
    public int FloorId;
    public string FloorName;
    public List<int> OpenDoor; 

    public FloorData(int floorId, string floorName, List<int> openDoor)
    {
        FloorId = floorId;
        FloorName = floorName;
        OpenDoor = openDoor;
    }
}
