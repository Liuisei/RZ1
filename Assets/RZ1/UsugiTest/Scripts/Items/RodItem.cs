using System;
using UnityEngine;
using Unity.Netcode;

public class RodItem : ItemBase
{
    [SerializeField] private GameObject magicBulletPrefab;
    [SerializeField] private Transform firePoint;
    private Camera cam;

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
        cam = transform.parent.GetComponentInChildren<Camera>();
        var bullet = Instantiate(magicBulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<MagicBullet>().Direction = cam.transform.forward;
    }
}