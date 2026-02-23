using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
    [SerializeField] private string url = "https://scp-wiki.wikidot.com/";

    public void Open()
    {
        Application.OpenURL(url);
    }
}
