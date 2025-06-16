using System.Collections.Generic;
using UnityEngine;

public class TaskFire : IBehaviorTask, IBehaviorNode
{
    private BulletLiu _bulletOBJ;
    private Transform _firePoint;
    private float _lifeTime = 3f; // 弾の自動消滅時間
    private float _bulletDamage = 10f; // 弾のダメージ
    private float _bulletSpeed = 10f; // 弾の速度
    private List<string> _targetTags;


    public TaskFire(BulletLiu bulletOBJ, Transform firePoint, List<string> targetTags, float lifeTime = 3f, float bulletDamage = 10f, float bulletSpeed = 10f)
    {
        _bulletOBJ = bulletOBJ;
        _firePoint = firePoint;
        _lifeTime = lifeTime;
        _bulletDamage = bulletDamage;
        _bulletSpeed = bulletSpeed;
        _targetTags = targetTags;
    }

    public void Enter()
    {
        var newBulletLiu = Object.Instantiate(_bulletOBJ, _firePoint.position, _firePoint.rotation);
        newBulletLiu.Speed = _bulletSpeed;
        newBulletLiu.Damage = _bulletDamage;
        newBulletLiu.LifeTime = _lifeTime;
        newBulletLiu.TargetTags = _targetTags;
    }

    public void Exit()
    {

    }
}
