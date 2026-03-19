using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance;

    [SerializeField] private List<InventoryItemType> items = new List<InventoryItemType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(InventoryItemType item)
    {
        items.Add(item);
        Debug.Log("Added to inventory: " + item);
    }

    public bool HasItem(InventoryItemType item)
    {
        return items.Contains(item);
    }

    public bool RemoveItem(InventoryItemType item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Removed from inventory: " + item);
            return true;
        }

        return false;
    }

    public List<InventoryItemType> GetItems()
    {
        return items;
    }

}