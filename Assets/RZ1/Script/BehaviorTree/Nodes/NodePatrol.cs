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

        public NodePatrol(NavMeshAgent agent, Transform[] points, bool autoBraking)
        {
            _agent = agent;
            _points = points;
            _autoBraking = autoBraking;
        }
        public void Enter()
        {
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
            // エージェントが現在設定された目標地点に行くように設定します
            _agent.destination = _points[_currentPoint].position;

            // 配列内の次の位置を目標地点に設定し、
            // 必要ならば出発地点にもどります
            _currentPoint = (_currentPoint + 1) % _points.Length;
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
