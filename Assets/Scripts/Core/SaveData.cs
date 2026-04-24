using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string sceneName;

    public float playerX;
    public float playerY;
    public float playerZ;
    public float playerRotY;
    public float cameraRotX;

    public float scp173X;
    public float scp173Y;
    public float scp173Z;

    //inventory data
    public List<int> inventoryItems = new List<int>(); // Store as ints for serialization
    public List<string> pickedUpItemIDs = new List<string>(); // IDs of items already collected
}