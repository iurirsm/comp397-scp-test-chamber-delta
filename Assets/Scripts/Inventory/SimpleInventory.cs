using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance;

    [SerializeField] private List<InventoryItemType> items = new List<InventoryItemType>();

    // Events to notify UI of changes
    public UnityEvent<InventoryItemType> OnItemAdded;
    public UnityEvent<InventoryItemType> OnItemRemoved;
    public UnityEvent OnInventoryChanged;

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
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
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
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public List<InventoryItemType> GetItems()
    {
        return items;
    }

    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
        OnInventoryChanged?.Invoke();
    }

}