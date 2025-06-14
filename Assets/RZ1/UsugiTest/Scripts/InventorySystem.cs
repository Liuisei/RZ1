using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Transform handSocket;
    [SerializeField] private Camera playerCamera;

    private List<ItemBase> inventory = new List<ItemBase>();
    private int currentIndex = -1;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 左クリック：拾う or 使用
        if (Input.GetMouseButtonDown(0))
        {
            if (currentIndex == -1)
            {
                TryPickUp();
            }
            else
            {
                inventory[currentIndex].Use();
            }
        }

        // ホイール回転：切り替え
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            SwitchItem(scroll > 0 ? 1 : -1);
        }

        // Qキー：捨てる
        if (Input.GetKeyDown(KeyCode.Q) && currentIndex != -1)
        {
            DropItem();
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
                inventory.Add(item);
                currentIndex = inventory.Count - 1;
                EquipCurrent();
            }
        }
    }

    private void EquipCurrent()
    {
        foreach (var item in inventory)
        {
            item.gameObject.SetActive(false);
        }

        if (currentIndex >= 0)
        {
            inventory[currentIndex].gameObject.SetActive(true);
            inventory[currentIndex].PickUp(handSocket, NetworkManager.Singleton.LocalClientId);
        }
    }

    private void SwitchItem(int direction)
    {
        if (inventory.Count == 0) return;
        currentIndex = (currentIndex + direction + inventory.Count) % inventory.Count;
        EquipCurrent();
    }

    private void DropItem()
    {
        ItemBase item = inventory[currentIndex];

        // 安全なドロップ位置計算
        Vector3 dropStart = handSocket.position + transform.forward * 1.0f + Vector3.up * 0.5f;

        // 真下にレイを飛ばして地面の高さを取得
        Vector3 dropPosition = dropStart;
        if (Physics.Raycast(dropStart, Vector3.down, out RaycastHit hit, 10f))
        {
            dropPosition = hit.point + Vector3.up * 1f; // 少し浮かせて落とす
        }

        item.Drop(dropPosition);

        inventory.RemoveAt(currentIndex);

        if (inventory.Count == 0)
            currentIndex = -1;
        else
            currentIndex %= inventory.Count;

        EquipCurrent();
    }
}
