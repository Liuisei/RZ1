using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

namespace RZ1.BehaviorTree
{
    public class NodeMoveTo : IBehaviorNode
    {
        private Rigidbody _aiRigidbody;
        private Vector3 _position;
        private bool _forcus;

        //private float _speed = 5f; // Speed of the AI movement, can be adjusted as needed
        private float _completDistance = 3f; // Distance to consider the movement complete
        private float _angleThreshold = 5f; // この角度以内なら回転しない

        public NodeMoveTo(Vector3 position, Rigidbody aiRigidbody, bool forcus = false)
        {
            _position = position;
            _aiRigidbody = aiRigidbody;
            _forcus = forcus;
            Debug.Log("NodeMoveTo created with position: " + _position + " and forcus: " + _forcus);
        }
        public void Enter()
        {
            Debug.Log("Entering NodeMoveTo with position: " + _position);

            if (_aiRigidbody == null)
            {
                Debug.LogError("AI Rigidbody is not assigned.");
                return;
            }

            MoveTo().Forget();
        }

        public async UniTask MoveTo()
        {
            await UniTask.DelayFrame(1);
            Vector3 direction = (_position - _aiRigidbody.position).normalized;
            _aiRigidbody.MovePosition(_aiRigidbody.position + direction * Time.deltaTime);
            if (CheckCompletion() == false)
            {
                Forcus(direction);
                MoveTo().Forget();
            }
            else
            {
                Exit();
            }
        }

        public void Forcus(Vector3 direction)
        {
            if (_forcus == false) return;
            if (direction == Vector3.zero) return;

            // 現在の向き
            Vector3 currentForward = _aiRigidbody.rotation * Vector3.forward;

            // 角度を求める
            float angle = Vector3.Angle(currentForward, direction);

            // 一定角度以内ならスキップ
            if (angle < _angleThreshold) return;

            // 回転する
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _aiRigidbody.rotation = Quaternion.Slerp(_aiRigidbody.rotation, targetRotation, Time.deltaTime * 5f);
        }

        public bool CheckCompletion()
        {
            float distance = Vector3.Distance(_aiRigidbody.position, _position);
            if (distance < _completDistance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void Exit()
        {
            Debug.Log("Exiting NodeMoveTo");
        }
    }
}
