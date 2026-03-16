using UnityEngine;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject minimap;
    [SerializeField] private AudioSource levelMusic;

    private bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If game over is visible, ignore pause input
            if (gameOverPanel != null && gameOverPanel.activeSelf)
                return;

            TogglePause();
        }
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

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}