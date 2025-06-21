using UnityEngine;
using System.Collections.Generic;

public class BulletLiu : MonoBehaviour
{
    public float Speed = 10f;      // 弾の速度
    public float Damage = 10f;     // 弾のダメージ
    public float LifeTime = 3f;    // 自動消滅までの時間

    // 衝突対象にするタグのリスト（例："Enemy", "Boss"など）
    public List<string> TargetTags;

    void Start()
    {
        Destroy(gameObject, LifeTime); // 一定時間後に消える
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * Speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // タグが対象のものかチェック
        if (TargetTags.Contains(other.tag))
        {
            Debug.Log($"Bullet hit: {other.name} with tag: {other.tag}");
        }

        // 何に当たっても弾は消える
        Destroy(gameObject);
    }
}
