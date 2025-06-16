using System;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class MagicBullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 20;
    private Vector3 Direction { get; set; }

    private float elapsedTime = 0f;

    private void Start()
    {
        if (IsServer)
        {
            // 初期化
            elapsedTime = 0f;
            Direction = FindObjectsByType<Camera>(sortMode: FindObjectsSortMode.None).First(x => x.isActiveAndEnabled).transform.forward;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        // 進行距離計算
        Vector3 move = Direction * (speed * Time.deltaTime);

        // RaycastAllでヒット判定（全部拾う）
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Direction, move.magnitude);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Debug.Log($"Hit: {hit.collider.gameObject.name} Tag: {hit.collider.tag}");

                if (hit.collider.TryGetComponent(out HealthSystem health))
                {
                    health.TakeDamageServerRpc(damage);
                    DespawnBullet();
                    return; // 最初のHealthSystemに当たった時点で終了
                }

                if (hit.collider.CompareTag("Field"))
                {
                    DespawnBullet();
                    return;
                }
            }
        }

        // 進行
        transform.position += move;

        // 時間経過で消滅
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifeTime)
        {
            DespawnBullet();
        }
    }

    private void DespawnBullet()
    {
        if (IsServer)
        {
            NetworkObject.Despawn();
        }
    }
}