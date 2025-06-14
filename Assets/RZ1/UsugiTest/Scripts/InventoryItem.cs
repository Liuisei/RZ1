using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _itemName; // アイテム名
    [SerializeField] private Image selectedIcon; // アイテムのアイコン

    public void SetItem(string itemName)
    {
        _itemName.text = itemName;
    }

    public void SetSelected(bool selected)
    {
        if (selectedIcon != null)
        {
            selectedIcon.gameObject.SetActive(selected);
        }
    }
}
