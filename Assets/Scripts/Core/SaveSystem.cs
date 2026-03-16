using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void SaveGame(Transform player, Transform scp173)
    {
        SaveData data = new SaveData();

        data.sceneName = SceneManager.GetActiveScene().name;

        if (player != null)
        {
            data.playerX = player.position.x;
            data.playerY = player.position.y;
            data.playerZ = player.position.z;
            data.playerRotY = player.eulerAngles.y;
        }

        if (scp173 != null)
        {
            data.scp173X = scp173.position.x;
            data.scp173Y = scp173.position.y;
            data.scp173Z = scp173.position.z;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Game saved to: " + savePath);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("Game loaded from: " + savePath);
        return data;
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }
}