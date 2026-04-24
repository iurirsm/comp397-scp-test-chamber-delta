using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scenes")]
    [SerializeField] private string menuScene = "01_Menu";
    [SerializeField] private string gameplayScene = "02_Gameplay";
    [SerializeField] private string winScreen = "03_WinScreen";
    [SerializeField] private float fadeDuration = 1f; //fade to black
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(menuScene);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        //clear inventory
        if (SimpleInventory.Instance != null)
            SimpleInventory.Instance.ClearInventory();

        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.ResetGameState();

        SceneManager.LoadScene(gameplayScene);
    }

    public void LoadWinScreen()
    {
        StartCoroutine(LoadWinScreenWithFade());
    }

    private IEnumerator LoadWinScreenWithFade()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ScreenFadeManager.Instance != null)
        {
            yield return StartCoroutine(ScreenFadeManager.Instance.FadeToBlackCoroutine(fadeDuration));
        }

        //load win scene after fade
        SceneManager.LoadScene(winScreen);
    }

    public void QuitGame()
    {
#if UNITY_WEBGL
        Debug.Log("Quit requested (WebGL cannot quit the browser tab).");
#else
        Debug.Log("exiting");
        Application.Quit();
#endif
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
}