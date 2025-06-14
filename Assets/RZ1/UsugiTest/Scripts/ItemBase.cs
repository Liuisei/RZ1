using Unity.Netcode;
using UnityEngine;

public abstract class ItemBase : NetworkBehaviour
{
    private Collider[] _colliders;
    private Renderer[] _renderers;
    private Rigidbody _rigidbody;
    private readonly Vector3 _itemHandPosition = new(0, 0.5f, 1);

    public abstract void Use();

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>(true);
        _renderers = GetComponentsInChildren<Renderer>(true);
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void PickUp(Transform handSocket)
    {
        transform.SetParent(handSocket);
        transform.localPosition = _itemHandPosition;
        transform.localRotation = Quaternion.identity;

        SetPhysics(false);
        SetVisible(true);
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;

        SetPhysics(true);
        SetVisible(true);
    }

    public void SetHidden(bool hidden)
    {
        if (hidden)
        {
            transform.position = new Vector3(0, -9999f, 0);  // 物理退避
        }
        SetVisible(!hidden);
    }

    private void SetPhysics(bool enable)
    {
        _rigidbody.isKinematic = !enable;
        foreach (var col in _colliders)
        {
            col.enabled = enable;
        }
    }

    private void SetVisible(bool visible)
    {
        foreach (var rend in _renderers)
        {
            rend.enabled = visible;
        }
    }
}