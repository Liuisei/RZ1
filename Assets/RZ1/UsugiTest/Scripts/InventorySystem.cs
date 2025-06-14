using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventorySystem : NetworkBehaviour
{
    [SerializeField] private Transform handSocket;
    [SerializeField] private Camera playerCamera;

    private NetworkList<NetworkObjectReference> networkInventory;
    private NetworkVariable<int> networkCurrentIndex = new NetworkVariable<int>(-1);

    void Awake()
    {
        networkInventory = new NetworkList<NetworkObjectReference>();
    }

    public override void OnNetworkSpawn()
    {
        networkInventory.OnListChanged += OnInventoryChanged;
        networkCurrentIndex.OnValueChanged += (prev, next) => EquipCurrent();
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (networkCurrentIndex.Value == -1)
            {
                TryPickUp();
            }
            else
            {
                UseItem();
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            SwitchItem(scroll > 0 ? 1 : -1);
        }

        if (Input.GetKeyDown(KeyCode.Q) && networkCurrentIndex.Value != -1)
        {
            DropItemServerRpc();
        }
    }

    private void TryPickUp()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            ItemBase item = hit.collider.GetComponent<ItemBase>();
            if (item != null)
            {
                PickupItemServerRpc(item.NetworkObject.NetworkObjectId);
            }
        }
    }

    [ServerRpc]
    private void PickupItemServerRpc(ulong networkObjectId)
    {
        var itemObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (itemObj.TryGetComponent<ItemBase>(out var item))
        {
            item.NetworkObject.ChangeOwnership(OwnerClientId);
            networkInventory.Add(item.NetworkObject);
            networkCurrentIndex.Value = networkInventory.Count - 1;
        }
    }

    private void UseItem()
    {
        if (networkCurrentIndex.Value < 0 || networkCurrentIndex.Value >= networkInventory.Count) return;

        if (networkInventory[networkCurrentIndex.Value].TryGet(out NetworkObject itemObj))
        {
            var item = itemObj.GetComponent<ItemBase>();
            item?.Use();
        }
    }

    private void EquipCurrent()
    {
        // クライアントは読み取りだけ
        if (!IsServer)
        {
            if (networkInventory.Count == 0 || networkCurrentIndex.Value >= networkInventory.Count)
            {
                return;
            }
        }
        else
        {
            // サーバーだけがindex補正・修正を行う
            if (networkInventory.Count == 0)
            {
                networkCurrentIndex.Value = -1;
                return;
            }
            if (networkCurrentIndex.Value >= networkInventory.Count)
            {
                networkCurrentIndex.Value = networkInventory.Count - 1;
            }
        }

        foreach (var reference in networkInventory)
        {
            if (reference.TryGet(out NetworkObject obj))
            {
                var item = obj.GetComponent<ItemBase>();
                item.SetHidden(true);
            }
        }

        if (networkCurrentIndex.Value >= 0)
        {
            var reference = networkInventory[networkCurrentIndex.Value];
            if (reference.TryGet(out NetworkObject obj))
            {
                var item = obj.GetComponent<ItemBase>();
                item.PickUp(handSocket);
            }
        }
    }


    private void SwitchItem(int direction)
    {
        if (networkInventory.Count == 0) return;

        var nextIndex = (networkCurrentIndex.Value + direction + networkInventory.Count) % networkInventory.Count;
        networkCurrentIndex.Value = nextIndex;
    }

    [ServerRpc]
    private void DropItemServerRpc()
    {
        if (networkCurrentIndex.Value < 0 || networkCurrentIndex.Value >= networkInventory.Count) return;

        if (networkInventory[networkCurrentIndex.Value].TryGet(out NetworkObject obj))
        {
            var item = obj.GetComponent<ItemBase>();

            Vector3 dropStart = handSocket.position + transform.forward * 1.0f + Vector3.up * 0.5f;
            Vector3 dropPosition = dropStart;

            if (Physics.Raycast(dropStart, Vector3.down, out RaycastHit hit, 10f))
            {
                dropPosition = hit.point + Vector3.up * 1f;
            }

            item.Drop(dropPosition);
            networkInventory.RemoveAt(networkCurrentIndex.Value);

            if (networkInventory.Count == 0)
                networkCurrentIndex.Value = -1;
            else
                networkCurrentIndex.Value %= networkInventory.Count;
        }
    }

    private void OnInventoryChanged(NetworkListEvent<NetworkObjectReference> change)
    {
        EquipCurrent();
    }
}
