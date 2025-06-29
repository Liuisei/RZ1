using Cysharp.Threading.Tasks;
using RZ1.BehaviorTree;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AC0 : NetworkBehaviour
{
    [SerializeField] NavMeshAgent _aiagent;
    [SerializeField] ESSVison _essVison;
    [SerializeField] Animator _animator;
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

        string[] targets = new string[] { "Player" };
        Debug.Log("AC0 is starting patrol and waiting for enemies...");
        _animator.Play("0Move");
        var detectedTag = await _essVison.WaitForAnyEnemyAsync(targets, cts.Token);
        if (!string.IsNullOrEmpty(detectedTag))
        {
            _currentNode.Exit();
            _currentNodeUpdate = null;
            _essVison.StopSearch();
            Foundenemy().Forget();
        }
    }

    async UniTask Foundenemy()
    {
        _animator.Play("0Atk1");
        // 現在のステートが終わるのを待つ
        await AnimatorUtility.WaitForAnimationToEnd(_animator, "0Atk1", 0);
        EndATK().Forget();
    }

    async UniTask EndATK()
    {
        _animator.Play("0Idle");
        await UniTask.Delay(2000);

        Defult().Forget();
    }




    private void Update()
    {
        if (!IsServer)
        {
            return; // Only run on the server
        }

        if (_currentNodeUpdate != null)
        {
            _currentNodeUpdate.Update();
        }
    }
}
