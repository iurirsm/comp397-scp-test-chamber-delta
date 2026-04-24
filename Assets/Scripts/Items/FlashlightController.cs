using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlightLight; // The light component
    
    // Define flashlight action with F key binding
    private InputAction flashlightAction = new InputAction("Flashlight", InputActionType.Button,
        binding: "<Keyboard>/f");
    
    private bool isFlashlightOn = false;

    private void OnEnable()
    {
        // Enable the flashlight action
        flashlightAction.Enable();
        // Subscribe to the press event
        flashlightAction.performed += OnFlashlightPerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe from the press event
        flashlightAction.performed -= OnFlashlightPerformed;
        // Disable the flashlight action
        flashlightAction.Disable();
    }

    // Callback when F is pressed
    private void OnFlashlightPerformed(InputAction.CallbackContext ctx)
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

    private void OnDestroy()
    {
        // Cleanup when script is destroyed
        flashlightAction.Dispose();
    }
}
