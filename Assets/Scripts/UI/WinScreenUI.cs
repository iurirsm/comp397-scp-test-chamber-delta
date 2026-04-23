using UnityEngine;

public class WinScreenUI : MonoBehaviour
{
    public void NewGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartNewGame();
    }

    public void LoadGame()
    {
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.LoadGame();
    }

    public void ExitGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }
}
