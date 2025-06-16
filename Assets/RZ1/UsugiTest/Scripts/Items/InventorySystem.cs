using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventorySystem : NetworkBehaviour
{
    [SerializeField] private Transform handSocket;
    [SerializeField] private Camera playerCamera;

    private const int MaxInventorySize = 10;
    private static readonly Vector3 HiddenPosition = new Vector3(0, -9999f, 0);
    private NetworkList<NetworkObjectReference> networkInventory = new();
    private NetworkVariable<int> networkCurrentIndex = new();

    public int CurrentIndex => networkCurrentIndex.Value;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < MaxInventorySize; i++)
            {
                networkInventory.Add(default);
            }
        }
        networkInventory.OnListChanged += OnInventoryChanged;
        networkCurrentIndex.OnValueChanged += (prev, next) => EquipCurrent();
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleInput();
    }

    public InventoryItemData GetSlot(int index)
    {
        if (index < 0 || index >= networkInventory.Count)
            return null;

        if (networkInventory[index].Equals(default))
            return null;

        if (networkInventory[index].TryGet(out NetworkObject obj))
        {
            return new InventoryItemData { ItemName = obj.name };
        }

        return null;
    }


    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 origin = playerCamera.transform.position;
            Vector3 direction = playerCamera.transform.forward;
            RequestPickUpServerRpc(origin, direction);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem();
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            SwitchItemRequestServerRpc(scroll > 0 ? 1 : -1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItemRequestServerRpc();
        }
    }

    [ServerRpc]
    private void RequestPickUpServerRpc(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 3f))
        {
            var itemBase = hit.collider.GetComponent<ItemBase>() ?? hit.collider.GetComponentInParent<ItemBase>();
            if (!itemBase) return;
            var targetSlot = networkCurrentIndex.Value;

            if (targetSlot < 0 || targetSlot >= networkInventory.Count)
            {
                networkCurrentIndex.Value = 0;
                targetSlot = 0;
            }

            if (networkInventory[targetSlot].Equals(default))
            {
                itemBase.NetworkObject.ChangeOwnership(OwnerClientId);
                networkInventory[targetSlot] = itemBase.NetworkObject;
            }
        }
    }

    private void UseItem()
    {
        if (networkCurrentIndex.Value < 0) return;
        if (networkInventory[networkCurrentIndex.Value].TryGet(out NetworkObject itemObj))
        {
            var item = itemObj.GetComponent<ItemBase>();
            item?.Use();
        }
    }

    private void EquipCurrent()
    {
        for (int i = 0; i < networkInventory.Count; i++)
        {
            if (networkInventory[i].TryGet(out NetworkObject obj))
            {
                var item = obj.GetComponent<ItemBase>();
                item.SetHidden(true);
                obj.transform.position = HiddenPosition;
            }
        }

        if (networkCurrentIndex.Value >= 0 &&
            networkCurrentIndex.Value < networkInventory.Count &&
            networkInventory[networkCurrentIndex.Value].TryGet(out NetworkObject activeObj))
        {
            var activeItem = activeObj.GetComponent<ItemBase>();
            activeItem.PickUp(handSocket);
        }
    }


    [ServerRpc]
    private void SwitchItemRequestServerRpc(int direction)
    {
        if (networkInventory.Count == 0) return;

        int nextIndex = (networkCurrentIndex.Value + direction + networkInventory.Count) % networkInventory.Count;
        networkCurrentIndex.Value = nextIndex;
    }

    [ServerRpc]
    private void DropItemRequestServerRpc()
    {
        if (networkCurrentIndex.Value < 0) return;
        if (!networkInventory[networkCurrentIndex.Value].TryGet(out NetworkObject obj)) return;

        var item = obj.GetComponent<ItemBase>();

        Vector3 dropStart = handSocket.position + transform.forward * 1.0f + Vector3.up * 0.5f;
        Vector3 dropPosition = dropStart;

        if (Physics.Raycast(dropStart, Vector3.down, out RaycastHit hit, 10f))
        {
            dropPosition = hit.point + Vector3.up * 1f;
        }

        item.Drop(dropPosition);
        obj.ChangeOwnership(NetworkManager.ServerClientId);
        networkInventory[networkCurrentIndex.Value] = default;
        networkCurrentIndex.Value = FindNextAvailableSlot();
    }

    private int FindNextAvailableSlot()
    {
        for (int i = 0; i < networkInventory.Count; i++)
        {
            if (!networkInventory[i].Equals(default)) return i;
        }
        return -1;
    }

    private void OnInventoryChanged(NetworkListEvent<NetworkObjectReference> change)
    {
        EquipCurrent();
    }
}

public class InventoryItemData
{
    public string ItemName;
}


