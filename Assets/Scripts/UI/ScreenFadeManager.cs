using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeManager : MonoBehaviour
{
    private static ScreenFadeManager instance;
    [SerializeField] private GameObject fadeImagePrefab;
    private Image activeFadeImage;

    public static ScreenFadeManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ScreenFadeManager>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IEnumerator FadeToBlackCoroutine(float duration)
    {
        if (fadeImagePrefab == null)
        {
            Debug.LogError("Fade image prefab not assigned!");
            yield break;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            yield break;
        }

        GameObject fadeObj = Instantiate(fadeImagePrefab, canvas.transform);
        activeFadeImage = fadeObj.GetComponent<Image>();

        if (activeFadeImage == null)
        {
            Debug.LogError("Fade prefab has no Image component!");
            Destroy(fadeObj);
            yield break;
        }

        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);
        activeFadeImage.color = startColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            activeFadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        activeFadeImage.color = endColor;
    }
}
