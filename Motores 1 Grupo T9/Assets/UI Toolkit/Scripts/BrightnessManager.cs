using UnityEngine;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    private const string BRIGHTNESS_KEY = "Brightness";

    // Por defecto 1f (brillo completo / claro)
    public float Brightness { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
    }

    public void SetBrightness(float value)
    {
        Brightness = value;

        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
        PlayerPrefs.Save();

        if (BrightnessController.Instance != null)
        {
            BrightnessController.Instance.ApplyBrightness(value);
        }
    }
}