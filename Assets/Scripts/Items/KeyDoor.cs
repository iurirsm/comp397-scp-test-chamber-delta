using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private Collider doorCollider;
    [SerializeField] private bool triggerWinOnTouch = true;
    [SerializeField] private AudioClip doorOpenSound; // Door opening sound

    private bool isUnlocked = false;
    private AudioSource audioSource;

    private void Start()
    {
        //trun off collider in the beggining
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

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
            StartCoroutine(PlaySoundAndLoadWinScreen());
        }
    }

    private IEnumerator PlaySoundAndLoadWinScreen()
    {
        //play door sound
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
            yield return new WaitForSeconds(doorOpenSound.length);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.LoadWinScreen();
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
