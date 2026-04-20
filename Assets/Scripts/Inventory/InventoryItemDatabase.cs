using UnityEngine;
using System.Collections.Generic;

public class InventoryItemDatabase : MonoBehaviour
{
    public static InventoryItemDatabase Instance { get; private set; }

    [System.Serializable]
    public class ItemData
    {
        public InventoryItemType itemType;
        public string itemName;
        public Sprite icon;
    }

    [SerializeField] private List<ItemData> items = new List<ItemData>();

    private Dictionary<InventoryItemType, ItemData> itemDatabase = new Dictionary<InventoryItemType, ItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build the database from the list
        foreach (ItemData item in items)
        {
            if (!itemDatabase.ContainsKey(item.itemType))
            {
                itemDatabase.Add(item.itemType, item);
            }
        }
    }

    public ItemData GetItemData(InventoryItemType itemType)
    {
        if (itemDatabase.ContainsKey(itemType))
        {
            return itemDatabase[itemType];
        }
        return null;
    }

    public string GetItemName(InventoryItemType itemType)
    {
        ItemData data = GetItemData(itemType);
        return data != null ? data.itemName : "Unknown";
    }

    public Sprite GetItemIcon(InventoryItemType itemType)
    {
        ItemData data = GetItemData(itemType);
        return data != null ? data.icon : null;
    }
}
