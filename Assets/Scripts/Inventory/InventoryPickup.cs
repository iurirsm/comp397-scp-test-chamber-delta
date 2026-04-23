using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickup : MonoBehaviour
{
    [SerializeField] private InventoryItemType itemType;
    [SerializeField] private string itemID; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (SimpleInventory.Instance == null)
        {
            Debug.LogError("SimpleInventory instance not found.");
            return;
        }

        SimpleInventory.Instance.AddItem(itemType);

        //track this item
        if (GameSaveManager.Instance != null && !string.IsNullOrEmpty(itemID))
        {
            GameSaveManager.Instance.RegisterPickedUpItem(itemID);
        }

        Destroy(gameObject);
    }

    public string GetItemID()
    {
        return itemID;
    }
}
