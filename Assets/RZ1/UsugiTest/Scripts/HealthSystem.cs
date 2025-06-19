using System;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // 体力のNetworkVariable（読み取りは全員可、書き込みはサーバーのみ）
    [SerializeField] private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public event Action OnDeath;
    public event Action OnRevive;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        // UI更新などがあればここに書く
        Debug.Log($"{OwnerClientId} の体力: {newValue}");

        if (newValue <= 0)
        {
            HandleDeath();
        }

        if (newValue > 0 && oldValue <= 0)
        {
            OnRevive?.Invoke();
            Debug.Log($"{OwnerClientId} が復活");
        }
    }

    private void HandleDeath()
    {
        Debug.Log($"{OwnerClientId} が死亡");
        // 死亡処理（例：リスポーン、復活など）
        OnDeath?.Invoke();
    }

    // クライアントからサーバーへダメージ申請
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (!IsServer) return;

        currentHealth.Value = Mathf.Max(0, currentHealth.Value - damage);
    }

    public int GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead => currentHealth.Value <= 0;
}