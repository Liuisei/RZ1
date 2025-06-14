using System;
using Unity.Netcode;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    private Collider _collider;
    private Rigidbody _rigidbody;
    private readonly Vector3 _itemHandPosition = new(0, 0.5f, 1);
    
    public abstract void Use();  // 使用時の効果

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void PickUp(Transform handSocket, ulong newOwnerId)
    {
        if (TryGetComponent<NetworkObject>(out var netObj))
        {
            netObj.ChangeOwnership(newOwnerId);
        }
        
        _collider.enabled = false;
        transform.SetParent(handSocket);
        transform.localPosition = _itemHandPosition;
        transform.localRotation = Quaternion.identity;

        _rigidbody.isKinematic = true;
    }


    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
    }
}