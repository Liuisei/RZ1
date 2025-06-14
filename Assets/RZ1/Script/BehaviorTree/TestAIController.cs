using UnityEngine;
using UnityEngine.UIElements;

namespace RZ1.BehaviorTree
{
    public class TestAIController : MonoBehaviour
    {
        [SerializeField] Transform _aiTransform;
        [SerializeField] Transform _targetTransform;
        [SerializeField] Rigidbody _aiRigidbody;

        private NodeMoveTo _nodeMoveTo;
        void Start()
        {
            _nodeMoveTo = new NodeMoveTo(_targetTransform.position, _aiRigidbody,true);
            _nodeMoveTo.Enter();

        }


    }

    
}