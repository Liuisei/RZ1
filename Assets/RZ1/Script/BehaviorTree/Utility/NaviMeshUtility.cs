using UnityEngine;
using UnityEngine.AI;

public class NaviMeshUtility
{
    public static bool IsGoal(NavMeshAgent agent)
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            Debug.Log("到達しました！");
            return true; // 目標地点に到達した場合
        }
        else
        {
            return false;
        }
    }
}
