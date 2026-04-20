using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    
    private InventoryItemType itemType;

    public void SetItem(InventoryItemType newItemType)
    {
        itemType = newItemType;
        
        if (InventoryItemDatabase.Instance != null)
        {
            InventoryItemDatabase.ItemData itemData = InventoryItemDatabase.Instance.GetItemData(itemType);
            
            if (itemData != null)
            {
                if (itemName != null)
                    itemName.text = itemData.itemName;
                    
                if (itemIcon != null && itemData.icon != null)
                    itemIcon.sprite = itemData.icon;
            }
        }
    }

    public InventoryItemType GetItemType()
    {
        return itemType;
    }
}
