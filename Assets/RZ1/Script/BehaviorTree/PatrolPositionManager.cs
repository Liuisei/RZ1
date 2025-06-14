using UnityEngine;

public class PatrolPositionManager : MonoBehaviour
{
    public static PatrolPositionManager I; // Singleton instance
    public Transform[] patrolPositions; // Array to hold patrol positions

    private void Awake()
    {
        I = this; // Set the singleton instance
    }
}
