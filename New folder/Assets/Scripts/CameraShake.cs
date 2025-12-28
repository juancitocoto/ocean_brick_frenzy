using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Shake(float duration, float magnitude)
    {
        // Simple placeholder: log. Replace with actual camera shake implementation.
        Debug.Log($"Camera shake: dur={duration}, mag={magnitude}");
    }
}
