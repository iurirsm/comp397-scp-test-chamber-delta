using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject mobileUIControls; // Mobile UI container
    [SerializeField] private AudioSource levelMusic;

    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    // Define pause action with Escape key binding
    private InputAction pauseAction = new InputAction("Pause", InputActionType.Button,
        binding: "<Keyboard>/escape");

    private bool isPaused;

    private void OnEnable()
    {
        // Enable the pause action
        pauseAction.Enable();
        // Subscribe to the press event
        pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe from the press event
        pauseAction.performed -= OnPausePerformed;
        // Disable the pause action
        pauseAction.Disable();
    }

    // Callback when Escape is pressed
    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        // If game over is visible, ignore pause input
        if (gameOverPanel != null && gameOverPanel.activeSelf)
            return;

        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) 
            {
                pausePanel.SetActive(true);
                minimap.SetActive(false);
            }

            if (levelMusic != null)
                levelMusic.Pause();

#if !UNITY_ANDROID
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
#else
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;            
#endif
            
            // Hide all mobile UI controls except the pause button
            HideMobileUIExceptPauseButton();
            
            OnPause?.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
                minimap.SetActive(true);                
            }

            if (levelMusic != null)
                levelMusic.UnPause();

#if !UNITY_ANDROID
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#else
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
#endif
            
            // Show all mobile UI controls
            ShowAllMobileUI();
            
            OnUnpause?.Invoke();
        }
    }

    private void OnDestroy()
    {
        // Cleanup when script is destroyed
        pauseAction.Dispose();
    }

    // Hide all children of mobileUIControls except the pause button
    private void HideMobileUIExceptPauseButton()
    {
        if (mobileUIControls == null)
            return;

        foreach (Transform child in mobileUIControls.transform)
        {
            if (child.CompareTag("PauseButton"))
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }
    }

    // Show all children of mobileUIControls
    private void ShowAllMobileUI()
    {
        if (mobileUIControls == null)
            return;

        foreach (Transform child in mobileUIControls.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}