using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private Collider doorCollider;
    [SerializeField] private bool triggerWinOnTouch = true;
    
    private bool isUnlocked = false;

    private void Start()
    {
        //trun off collider in the beggining
        if (doorCollider != null)
            doorCollider.enabled = false;

        //subscribe to inventory events to check for key
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.AddListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.AddListener(OnItemRemoved);
            
            // Check if key already exists in inventory (in case door loads after pickup)
            if (SimpleInventory.Instance.HasItem(InventoryItemType.Key))
                UnlockDoor();
        }
    }

    private void OnDestroy()
    {
        //unsubscribe from events
        if (SimpleInventory.Instance != null)
        {
            SimpleInventory.Instance.OnItemAdded.RemoveListener(OnItemAdded);
            SimpleInventory.Instance.OnItemRemoved.RemoveListener(OnItemRemoved);
        }
    }

    private void OnItemAdded(InventoryItemType itemType)
    {
        if (itemType == InventoryItemType.Key)
            UnlockDoor();
    }

    private void OnItemRemoved(InventoryItemType itemType)
    {
        if (itemType == InventoryItemType.Key)
            LockDoor();
    }

    private void UnlockDoor()
    {
        if (doorCollider != null)
            doorCollider.enabled = true;
        
        isUnlocked = true;
        Debug.Log("Door unlocked! You can now escape.");
    }

    private void LockDoor()
    {
        if (doorCollider != null)
            doorCollider.enabled = false;
        
        isUnlocked = false;
        Debug.Log("Door locked.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked || !other.CompareTag("Player"))
            return;

        Debug.Log("Escaping through the door!");

        if (triggerWinOnTouch)
        {
            Debug.Log("You escaped! Level Complete!");
            GameManager.Instance.LoadWinScreen();
        }
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
