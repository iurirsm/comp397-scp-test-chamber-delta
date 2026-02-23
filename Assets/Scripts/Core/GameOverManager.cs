using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;   
    [SerializeField] private AudioClip deathClip;

    [Header("Scenes (optional)")]
    [SerializeField] private string mainMenuSceneName = "01_Menu";

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

        // Cursor for WebGL/UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
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
}