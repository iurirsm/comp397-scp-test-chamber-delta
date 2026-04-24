using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject mobileUIControls; // Mobile UI container

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;   
    [SerializeField] private AudioClip deathClip;

    [Header("Scenes (optional)")]
    [SerializeField] private string mainMenuSceneName = "01_Menu";

    public UnityEvent OnGameOver;

    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop level music
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in allAudio)
        {
            if (a.loop) 
                a.Stop();
        }

        // Play death SFX
        if (sfxSource != null && deathClip != null)
            sfxSource.PlayOneShot(deathClip);

        // Pause gameplay
        Time.timeScale = 0f;

        // Cursor for WebGL/UI (not on Android)
// #if !UNITY_ANDROID
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
// #else
//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;
// #endif

        // Show UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            minimap.SetActive(false);            
        }
        
        // Hide all mobile UI controls
        HideAllMobileUI();
        
        OnGameOver?.Invoke();
            
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hide all mobile UI controls
    private void HideAllMobileUI()
    {
        if (mobileUIControls == null)
            return;

        mobileUIControls.SetActive(false);
    }
}