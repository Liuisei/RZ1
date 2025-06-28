using Cysharp.Threading.Tasks;
using RZ1.BehaviorTree;
using System;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AC0 : NetworkBehaviour
{
    [SerializeField] NavMeshAgent _aiagent;
    [SerializeField] ESSVison _essVison;
    private IBehaviorUpdate _currentNodeUpdate;
    private IBehaviorNode _currentNode;

    private NodePatrol _nodePatrol;

    private void Start()
    {
        if (_aiagent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned in AC0.");
        }

        if (_essVison == null)
        {
            Debug.LogError("ESSVison is not assigned in AC0.");
            return;
        }

        if (IsServer)
        {
            Defult().Forget();
        }
    }
    private async UniTask Defult()
    {
        _essVison.StartSearch();
        var cts = new CancellationTokenSource();


        _nodePatrol = new NodePatrol(_aiagent, PatrolPositionManager.I.patrolPositions, true, true);
        _nodePatrol.Enter();
        _currentNodeUpdate = _nodePatrol;
        _currentNode = _nodePatrol;
    }
}
