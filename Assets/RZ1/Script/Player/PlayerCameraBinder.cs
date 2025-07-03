using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraBinder : NetworkBehaviour
{
    [SerializeField] private Transform followTarget;

    public override void OnNetworkSpawn()
    {
        Debug.Log(IsOwner);
        if (!IsOwner) return;

        var virtualCam = FindAnyObjectByType<CinemachineCamera>();
        if (virtualCam)
        {
            virtualCam.Follow = followTarget;
            virtualCam.LookAt = followTarget;
        }
    }
}

