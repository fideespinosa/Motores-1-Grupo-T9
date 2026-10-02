using UnityEngine;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance;

    private const string BRIGHTNESS_KEY = "Brightness";

    public float Brightness { get; private set; } = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 0f);
    }

    public void SetBrightness(float value)
    {
        Brightness = value;

        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
        PlayerPrefs.Save();
    }
}
