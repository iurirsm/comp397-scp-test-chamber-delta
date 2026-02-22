using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Scene Names (must match Build Settings)")]
    [SerializeField] private string gameplayScene = "02_Gameplay";

    public void NewGame()
    {
        SceneManager.LoadScene(gameplayScene);
    }

    public void Quit()
    {
#if UNITY_WEBGL
        Debug.Log("Quit requested (WebGL can’t close the tab).");
#else
        Application.Quit();
#endif
    }
}