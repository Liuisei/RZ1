// 攻撃基底クラス

using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [SerializeField] protected List<Transform> _hitBoxPoints;
    [SerializeField] protected int _startFrame = 0;
    [SerializeField] protected int _endFrame = int.MaxValue;
    [SerializeField] protected float _radius = 0.1f;
    public abstract bool CanAttack();
    public abstract void StartAttack();
}