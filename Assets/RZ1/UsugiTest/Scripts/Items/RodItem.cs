using UnityEngine;
using Unity.Netcode;

public class RodItem : ItemBase
{
    [SerializeField] private GameObject magicBulletPrefab;
    [SerializeField] private Transform firePoint;

    public override void Use()
    {
        if (IsOwner)
        {
            ShootRequestServerRpc();
        }
    }

    [ServerRpc]
    private void ShootRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        var bullet = Instantiate(magicBulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}