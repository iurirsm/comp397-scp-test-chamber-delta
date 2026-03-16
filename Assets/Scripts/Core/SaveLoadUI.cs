using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadUI : MonoBehaviour
{
    public void SaveGame()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
        }
        else
        {
            Debug.LogError("GameSaveManager instance not found.");
        }
    }

    public void LoadGame()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("GameSaveManager instance not found.");
        }
    }
}