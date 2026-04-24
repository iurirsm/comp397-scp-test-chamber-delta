using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryItemPrefab; //prefab to instantiate for each item
    [SerializeField] private GameObject inventoryPanel; //panel to show/hide on pause
    
    private void Start()
    {
        RefreshInventoryDisplay();
        
        // Subscribe to inventory events
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.AddListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.AddListener(OnItemRemoved);
        }
        
        // Subscribe to pause events
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.OnPause.AddListener(OnGamePaused);
            pauseManager.OnUnpause.AddListener(OnGameUnpaused);
        }
        
        // Subscribe to game over events
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.OnGameOver.AddListener(OnGameOver);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from inventory events
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.RemoveListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.RemoveListener(OnItemRemoved);
        }
        
        // Unsubscribe from pause events
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.OnPause.RemoveListener(OnGamePaused);
            pauseManager.OnUnpause.RemoveListener(OnGameUnpaused);
        }
        
        // Unsubscribe from game over events
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.OnGameOver.RemoveListener(OnGameOver);
        }
    }

    private void OnItemAdded(InventoryItemType itemType)
    {
        AddItemToUI(itemType);
    }

    private void OnItemRemoved(InventoryItemType itemType)
    {
        RemoveItemFromUI(itemType);
    }
    
    private void OnGamePaused()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
    
    private void OnGameUnpaused()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }
    
    private void OnGameOver()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void RefreshInventoryDisplay()
    {
        //clear existing UI
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        //add all current items
        if (SimpleInventory.Instance != null)
        {
            foreach (InventoryItemType item in SimpleInventory.Instance.GetItems())
            {
                AddItemToUI(item);
            }
        }
    }

    private void AddItemToUI(InventoryItemType itemType)
    {
        GameObject itemUI = Instantiate(inventoryItemPrefab, inventoryContainer);
        InventoryItemUI itemUIScript = itemUI.GetComponent<InventoryItemUI>();
        
        if (itemUIScript != null)
        {
            itemUIScript.SetItem(itemType);
        }
        else {Debug.LogError("No item like that found");}
    }

    private void RemoveItemFromUI(InventoryItemType itemType)
    {
        foreach (Transform child in inventoryContainer)
        {
            InventoryItemUI itemUI = child.GetComponent<InventoryItemUI>();
            if (itemUI != null && itemUI.GetItemType() == itemType)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
}
