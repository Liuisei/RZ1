using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RZ1.BehaviorTree
{
    public class TestAIController : MonoBehaviour
    {
        [SerializeField] NavMeshAgent _aiagent;
        [SerializeField] Transform _aiTransform;
        [SerializeField] Rigidbody _aiRigidbody;

        private NodePatrol _nodePatrol;
        private IBehaviorNodeUpdate _currentNodeUpdate;

        void Start()
        {
            _aiagent = GetComponent<NavMeshAgent>();
            _aiTransform = GetComponent<Transform>();
            _aiRigidbody = GetComponent<Rigidbody>();
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
            RayCastVision();
        }

        private void OnDrawGizmosSelected()
        {
            if (_aiTransform == null)
            {
                return;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_aiTransform.position, 0.5f);
            RayCastVision();
        }

        private void RayCastVision()
        {
            if (Physics.Raycast(_aiTransform.position, transform.forward, out RaycastHit hit, distance))
            {
                Debug.DrawLine(_aiTransform.position, hit.point, Color.green);
                Debug.Log("Hit: " + hit.collider.name);
            }
            else
            {
                Debug.DrawRay(_aiTransform.position, transform.forward * distance, rayColor);
            }
        }

        private float distance = 10f;
        private Color rayColor = Color.green;
    }

    
}