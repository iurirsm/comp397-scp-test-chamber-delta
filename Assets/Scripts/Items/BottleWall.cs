using UnityEngine;

public class BottleWall : MonoBehaviour
{
    private void Start()
    {
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.AddListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.AddListener(OnItemRemoved);

            if (SimpleInventory.Instance.HasItem(InventoryItemType.SCP500))
                DisableWall();
            else
                EnableWall();
        }
    }

    private void OnDestroy()
    {
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.RemoveListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.RemoveListener(OnItemRemoved);
        }
    }

    private void OnItemAdded(InventoryItemType itemType)
    {
        if (itemType == InventoryItemType.SCP500)
            DisableWall();
    }

    private void OnItemRemoved(InventoryItemType itemType)
    {
        if (itemType == InventoryItemType.SCP500)
            EnableWall();
    }

    private void DisableWall()
    {
        gameObject.SetActive(false);
        Debug.Log("BottleWall disabled - player has pills");
    }

    private void EnableWall()
    {
        gameObject.SetActive(true);
        Debug.Log("BottleWall enabled - player does not have pills");
    }
}
