using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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
        private IBehaviorUpdate _currentNodeUpdate;
        private IBehaviorNode _currentNode;

        [Header("Bullet")]
        [SerializeField] BulletLiu _bulletPrefab;
        [SerializeField] Transform _bulletSpawnPoint;
        [SerializeField] float _bulletSpeed = 10f;
        [SerializeField] float _bulletLifeTime = 3f;
        [SerializeField] float _bulletDamage = 10f;

        NodePatrol _nodePatrol;

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


            _nodePatrol = new NodePatrol(_aiagent, PatrolPositionManager.I.patrolPositions, false);
            _nodePatrol.Enter();
            _currentNodeUpdate = _nodePatrol;
            _currentNode = _nodePatrol;

            string[] targets = new string[] { "Player" };
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
            TaskFire taskFire = new TaskFire(_bulletPrefab, _bulletSpawnPoint, new List<string> { "Player" }, _bulletLifeTime, _bulletDamage, _bulletSpeed);
            taskFire.Enter();
            await UniTask.Delay(1000); // Simulate some delay for firing
            Defult().Forget();
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