using UnityEngine;

public class MobileUIController : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Enable mobile UI on Android/iOS
        gameObject.SetActive(true);
#else
        // Disable mobile UI on desktop
        gameObject.SetActive(false);
#endif
    }
}
