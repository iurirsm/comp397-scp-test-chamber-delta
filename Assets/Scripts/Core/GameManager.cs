using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scenes")]
    [SerializeField] private string menuScene = "01_Menu";
    [SerializeField] private string gameplayScene = "02_Gameplay";
    [SerializeField] private string gameOverScene = "03_GameOver";

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // If in Bootstrap, jump to Menu.
        // If you press Play from another scene, leave it alone.
        string active = SceneManager.GetActiveScene().name;
        if (active == "00_Bootstrap")
        {
            LoadMenu();
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(menuScene);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(gameplayScene);
    }

    public void LoadGameOver()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(gameOverScene);
    }

    public void QuitGame()
    {
#if UNITY_WEBGL
        Debug.Log("Quit requested (WebGL cannot quit the browser tab).");
#else
        Application.Quit();
#endif
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
}