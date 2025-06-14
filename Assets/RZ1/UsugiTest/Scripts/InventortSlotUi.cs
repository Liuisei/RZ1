using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySlotUi : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> inventoryItems = new List<InventoryItem>(10);
    [SerializeField] private InventorySystem inventorySystem;

    private void Start()
    {
        inventorySystem = FindObjectsByType<InventorySystem>(FindObjectsSortMode.None).First(x => x.IsOwner);
    }

    void Update()
    {
        // インベントリの状態を毎フレーム反映
        UpdateInventoryUi();
    }

    private void UpdateInventoryUi()
    {
        if (inventorySystem == null || inventoryItems.Count == 0)
        {
            return; // インベントリシステムが未設定またはアイテムがない場合は何もしない
        }
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            // スロット内のデータ取得
            var slot = inventorySystem.GetSlot(i);

            // スロットが空なら空表示
            if (slot == null)
            {
                inventoryItems[i].SetItem("");
            }
            else
            {
                inventoryItems[i].SetItem(slot.ItemName); // ←アイテム名をセット
            }

            // 選択中ならハイライト
            inventoryItems[i].SetSelected(inventorySystem.CurrentIndex == i);
        }
    }
}