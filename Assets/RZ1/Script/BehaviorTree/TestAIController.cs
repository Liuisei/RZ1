using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RZ1.BehaviorTree
{
    public class TestAIController : MonoBehaviour
    {
        [SerializeField] NavMeshAgent _aiagent;
        [SerializeField] ESSVison _essVison;
        private IBehaviorNodeUpdate _currentNodeUpdate;

        void Start()
        {
            if (_essVison == null)
            {
                Debug.LogError("ESSVison is not assigned in TestAIController.");
                return;
            }
            if (PatrolPositionManager.I == null)
            {
                Debug.LogError("PatrolPositionManager is not assigned in TestAIController.");
                return;
            }

            _aiagent = GetComponent<NavMeshAgent>();

            Defult().Forget();
        }

        async UniTask Defult()
        {
            _essVison.StartSearch();
            var cts = new CancellationTokenSource();

            NodePatrol nodePatrol = new NodePatrol(_aiagent, PatrolPositionManager.I.patrolPositions, false);
            nodePatrol.Enter();
            _currentNodeUpdate = nodePatrol;
            string[] targets = new string[] { "Player" };
            var detectedTag = await _essVison.WaitForAnyEnemyAsync(targets, cts.Token);
            if (!string.IsNullOrEmpty(detectedTag))
            {
                _currentNodeUpdate.Exit();
                _currentNodeUpdate = null;
                _essVison.StopSearch();
                Foundenemy();
            }
        }

        void Foundenemy()
        {
            Debug.Log("Enemy found! Switching behavior.");

        }

        private void Update()
        {
            if (_currentNodeUpdate != null)
            {
                _currentNodeUpdate.Update();
            }
        }
    }


}