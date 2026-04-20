using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlightLight; // The light component
    
    private bool isFlashlightOn = false;

    private void Update()
    {
        // Check if F key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Check if player has flashlight in inventory
            if (SimpleInventory.Instance != null && SimpleInventory.Instance.HasItem(InventoryItemType.Flashlight))
            {
                ToggleFlashlight();
            }
            else
            {
                Debug.Log("You don't have a flashlight!");
            }
        }
    }

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;
        
        if (flashlightLight != null)
            flashlightLight.enabled = isFlashlightOn;
        
        Debug.Log("Flashlight turned " + (isFlashlightOn ? "ON" : "OFF"));
    }

    public bool IsFlashlightOn()
    {
        return isFlashlightOn;
    }
}
