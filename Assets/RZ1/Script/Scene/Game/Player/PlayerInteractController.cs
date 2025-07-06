using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractController : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactableLayer;

    private InteractableBase _currentTarget;

    private void Start()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    void Update()
    {
        if (!IsOwner) return;

        CheckForInteractable();

        if (NetworkInputHandler.TryGetInput(OwnerClientId, out var input))
        {
            if (input.IsButtonPressed(NetworkInputHandler.InputButton.Interact) && _currentTarget)
            {
                RequestInteractServerRpc(_currentTarget.NetworkObject.NetworkObjectId);
            }
        }
    }

    private void CheckForInteractable()
    {
        _currentTarget = null;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactableLayer))
        {
            _currentTarget = hit.collider.GetComponent<InteractableBase>();
        }
        Debug.DrawRay(_camera.transform.position, _camera.transform.forward * _interactDistance, Color.red);
    }

    [ServerRpc]
    private void RequestInteractServerRpc(ulong targetNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out var netObj))
        {
            var interactable = netObj.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                interactable.Interact(OwnerClientId);
            }
        }
    }
}