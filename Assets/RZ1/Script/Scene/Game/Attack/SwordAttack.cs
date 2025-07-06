using System.Collections;
using UnityEngine;

public class SwordAttack : AttackBase
{
    private bool _isAttacking = false;
    private int _currentFrame = 0;
    private Coroutine _attackRoutine;

    private const int MaxHitCount = 10;
    private readonly Collider[] _hitResults = new Collider[MaxHitCount];

    public override bool CanAttack()
    {
        return !_isAttacking;
    }

    public override void StartAttack()
    {
        if (_isAttacking) return;
        _attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _currentFrame = 0;

        while (_currentFrame <= _endFrame)
        {
            if (_currentFrame >= _startFrame)
            {
                DoHitDetection();
            }

            _currentFrame++;
            yield return null;
        }

        _isAttacking = false;
    }

    private void DoHitDetection()
    {
        for (var i = 0; i < _hitBoxPoints.Count; i++)
        {
            if (!_hitBoxPoints[i]) continue;
            if (i != _hitBoxPoints.Count - 1)
            {
                DebugDrawUtility.DrawWireCapsule(_hitBoxPoints[i].position, _hitBoxPoints[i + 1].position, _radius, Color.red);
            }

            int hitCount = Physics.OverlapCapsuleNonAlloc(_hitBoxPoints[i].position, _hitBoxPoints[i + 1].position, _radius, _hitResults);

            for (int j = 0; j < hitCount; j++)
            {
                var hit = _hitResults[j];

                if (hit.CompareTag("Enemy"))
                {
                    Debug.Log($"Hit enemy: {hit.name}");
                    // TODO: ダメージ処理など
                }
            }
        }
    }
}