using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ResourceManagement;

namespace RZ1.BehaviorTree
{
    [Serializable]
    public class NodePatrol : IBehaviorNode, IBehaviorUpdate
    {

        private Transform[] _points; // パトロール地点の配列
        private NavMeshAgent _agent;
        private bool _autoBraking = false; // 自動ブレーキを無効にするためのフラグ
        private int _currentPoint = 0; // 現在の目標地点のインデックス
        private bool _useRandom = false; // ランダムか順番か

        public NodePatrol(NavMeshAgent agent, Transform[] points, bool autoBraking, bool useRandom = false)
        {
            _agent = agent;
            _points = points;
            _autoBraking = autoBraking;
            _useRandom = useRandom;
        }
        public void Enter()
        {
            _agent.isStopped = false;

            Debug.Log("Entering NodePatrol with " + _points.Length + " points.");
            if (_points.Length == 0)
            {
                Debug.LogWarning("No patrol points set. Exiting patrol node.");
                return;
            }
            _agent.autoBraking = _autoBraking; // 自動ブレーキの設定
            GotoNextPoint();
        }



        void GotoNextPoint()
        {
            if (_points.Length == 0) return;

            if (_useRandom)
            {
                int nextPoint;
                do
                {
                    nextPoint = UnityEngine.Random.Range(0, _points.Length);
                } while (nextPoint == _currentPoint && _points.Length > 1); // 同じ場所を避ける
                _currentPoint = nextPoint;
            }
            else
            {
                _currentPoint = (_currentPoint + 1) % _points.Length;
            }

            _agent.destination = _points[_currentPoint].position;
        }


        public void Exit()
        {
            _agent.isStopped = true;
        }

        public void Update()
        {
            if (NaviMeshUtility.IsGoal(_agent) == true)
            {
                GotoNextPoint();
            }
        }
    }
}
