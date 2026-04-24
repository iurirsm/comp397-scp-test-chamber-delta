using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    private Transform player;
    private Transform scp173;
    private SaveData loadedData;
    private HashSet<string> pickedUpItems = new HashSet<string>(); // Track picked up items

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterPickedUpItem(string itemID)
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            pickedUpItems.Add(itemID);
            Debug.Log("Registered picked up item: " + itemID);
        }
    }

    public void ResetGameState()
    {
        pickedUpItems.Clear();
        Debug.Log("Game state reset - picked up items cleared.");
    }

    public void SaveGame()
    {
        FindReferences();

        if (player == null)
        {
            Debug.LogError("Player not found. Tag the real player object as Player.");
            return;
        }

        if (scp173 == null)
        {
            Debug.LogError("SCP-173 not found. Tag the real SCP object as SCP173.");
            return;
        }

        //get inventory from SimpleInventory
        List<InventoryItemType> inventory = new List<InventoryItemType>();
        if (SimpleInventory.Instance != null)
        {
            inventory = SimpleInventory.Instance.GetItems();
        }

        SaveSystem.SaveGame(player, scp173, inventory, pickedUpItems.ToList());
    }

    public void LoadGame()
    {
        loadedData = SaveSystem.LoadGame();

        if (loadedData == null)
            return;

        Time.timeScale = 1f;

        StartCoroutine(LoadAndRestore());
    }

    private IEnumerator LoadAndRestore()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(loadedData.sceneName);
        yield return loadOp;

        yield return null;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);

        FindReferences();

        if (player == null)
        {
            Debug.LogError("Player not found after loading.");
        }
        else
        {
            RestorePlayerPosition();
        }

        if (scp173 == null)
        {
            Debug.LogError("SCP-173 not found after loading.");
        }
        else
        {
            RestoreScpPosition();
        }

        RestoreInventory();

        RemovePickedUpItems();

        loadedData = null;
    }

    private void RestoreInventory()
    {
        if (SimpleInventory.Instance == null)
        {
            Debug.LogError("SimpleInventory not found during load.");
            return;
        }

        //clear current inventory
        List<InventoryItemType> currentItems = new List<InventoryItemType>(SimpleInventory.Instance.GetItems());
        foreach (var item in currentItems)
        {
            SimpleInventory.Instance.RemoveItem(item);
        }

        //restore saved inventory
        foreach (int itemInt in loadedData.inventoryItems)
        {
            InventoryItemType item = (InventoryItemType)itemInt;
            SimpleInventory.Instance.AddItem(item);
        }

        pickedUpItems.Clear();
        foreach (string itemID in loadedData.pickedUpItemIDs)
        {
            pickedUpItems.Add(itemID);
        }

        Debug.Log("Inventory restored with " + loadedData.inventoryItems.Count + " items.");
    }

    private void RemovePickedUpItems()
    {
        InventoryPickup[] allPickups = FindObjectsOfType<InventoryPickup>();

        foreach (InventoryPickup pickup in allPickups)
        {
            string itemID = pickup.GetItemID();
            if (!string.IsNullOrEmpty(itemID) && pickedUpItems.Contains(itemID))
            {
                Debug.Log("Removing already picked up item: " + itemID);
                Destroy(pickup.gameObject);
            }
        }
    }

    private void RestorePlayerPosition()
    {
        Vector3 targetPos = new Vector3(
            loadedData.playerX,
            loadedData.playerY,
            loadedData.playerZ
        );

        float targetYRotation = loadedData.playerRotY;

        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (cc != null)
            cc.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        player.position = targetPos;
        player.rotation = Quaternion.Euler(0f, targetYRotation, 0f);

        if (rb != null)
            rb.isKinematic = false;

        if (cc != null)
            cc.enabled = true;

        Debug.Log("Player restored to: " + targetPos + " | Y Rotation: " + targetYRotation);
    }

    private void RestoreScpPosition()
    {
        Vector3 targetPos = new Vector3(
            loadedData.scp173X,
            loadedData.scp173Y,
            loadedData.scp173Z
        );

        NavMeshAgent agent = scp173.GetComponent<NavMeshAgent>();
        Rigidbody rb = scp173.GetComponent<Rigidbody>();

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        scp173.position = targetPos;

        if (rb != null)
            rb.isKinematic = false;

        if (agent != null)
            agent.enabled = true;

        if (agent != null && agent.isOnNavMesh)
            agent.Warp(targetPos);

        Debug.Log("SCP-173 restored to: " + targetPos);
    }

    private void FindReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        GameObject scpObject = GameObject.FindGameObjectWithTag("SCP173");
        if (scpObject != null)
            scp173 = scpObject.transform;
    }
}