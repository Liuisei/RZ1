using System;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class RodItem : ItemBase
{
    [SerializeField] private GameObject magicBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float cooldownTime = 1.5f; // クールタイム秒数

    private Camera cam;
    private float lastUseTime = -Mathf.Infinity;

    private void Start()
    {
        var cams = FindObjectsByType<CameraLook>(FindObjectsSortMode.None);
        cam = cams.First( x => x.IsOwner).GetComponent<Camera>();
    }

    public override void Use()
    {
        if (!IsOwner) return;

        if (Time.time - lastUseTime < cooldownTime)
        {
            Debug.Log("クールタイム中...");
            return;
        }

        lastUseTime = Time.time;
        if (cam == null)
            cam = GetComponentInChildren<Camera>();

        // カメラの向きをサーバーに渡す
        ShootRequestServerRpc(cam.transform.forward);
    }

    [ServerRpc]
    private void ShootRequestServerRpc(Vector3 shootDirection, ServerRpcParams rpcParams = default)
    {
        var bullet = Instantiate(magicBulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<MagicBullet>().Direction = shootDirection;
    }
}