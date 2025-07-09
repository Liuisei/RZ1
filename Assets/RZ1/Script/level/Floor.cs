using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public int FloorId;
    public string FloorName;
    [SerializeField] private List<Door> _openDoors;
    public GuildType GuildType;

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
}



