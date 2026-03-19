using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickup : MonoBehaviour
{
    [SerializeField] private InventoryItemType itemType;

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
        Destroy(gameObject);
    }
}
