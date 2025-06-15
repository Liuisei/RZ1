using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RZ1.BehaviorTree
{
    public class TestAIController : MonoBehaviour
    {
        [SerializeField] NavMeshAgent _aiagent;
        private IBehaviorNodeUpdate _currentNodeUpdate;

        void Start()
        {
            _aiagent = GetComponent<NavMeshAgent>();
            if (PatrolPositionManager.I == null)
            {
                return;
            }

            NodePatrol nodePatrol = new NodePatrol(_aiagent, PatrolPositionManager.I.patrolPositions, false);
            nodePatrol.Enter();
            _currentNodeUpdate = nodePatrol;
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