//using GLTFast.Schema;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class MenuPauseLevel1 : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelPause;

    [Header("HudDron")]
    [SerializeField] private CamDroneUI UIScript;

    [Header("Opciones")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider fxSlider;
    public Slider brightnessSlider;


    public static bool gamePaused = false;
    private bool openOption = false;
    private bool openPause = false;
    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged); //el slider al cambiar de valor, llama a la funcion ChangeVolumenMaster
        fxSlider.onValueChanged.AddListener(OnFxChanged);
        brightnessSlider.onValueChanged.AddListener(OnSliderChanged);
    }
    private void Start()
    {
        // 1. Cargar y sincronizar Volúmenes (AudioMixer -> Sliders)
        if (audioMixer != null)
        {
            if (musicSlider != null && audioMixer.GetFloat("VolMusic", out float musicDB))
            {
                musicSlider.value = DBToNormalized(musicDB);
            }

            if (fxSlider != null && audioMixer.GetFloat("VolFX", out float fxDB))
            {
                fxSlider.value = DBToNormalized(fxDB);
            }
        }

        // 2. Cargar y sincronizar Brillo (GlobalBrightnessManager -> Slider)
        if (brightnessSlider != null && GlobalBrightnessManager.Instance != null)
        {
            brightnessSlider.value = GlobalBrightnessManager.Instance.GetNormalizedBrightness();
        }
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P) || Keyboard.current.pKey.isPressed)
        {
            Pause();
            
        }
    }
    public void Pause()
    {
        UIScript.SetVisibility(false);

        if (Cursor.visible && Cursor.lockState == CursorLockMode.None)
        {
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        gamePaused = true;
        openPause = true;


        if (panelPause != null)
        { panelPause.SetActive(true); }

        Time.timeScale = 0f;

        if (brightnessSlider != null && GlobalBrightnessManager.Instance != null)
        {
            brightnessSlider.value = GlobalBrightnessManager.Instance.GetNormalizedBrightness();
        }
        if (audioMixer != null)
        {
            if (musicSlider != null && audioMixer.GetFloat("VolMusic", out float musicDB))
            {
                musicSlider.value = DBToNormalized(musicDB);
            }
            if (fxSlider != null && audioMixer.GetFloat("VolFX", out float fxDB))
            {
                fxSlider.value = DBToNormalized(fxDB);
            }
        }
    }

    public void Continue()
    {
        UIScript.SetVisibility(true);

        gamePaused = false;
        if (panelPause != null)
        { panelPause.SetActive(false); }

        Time.timeScale = 1f;

        // Ocultar y volver a bloquear el cursor para jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        gamePaused = false;

        
        SceneManager.LoadScene("MainMenu OK");
    }
    public void OnSliderChanged(float newValue)
    {
        // Le enviamos el valor puro (0 a 1) al manager global
        if (GlobalBrightnessManager.Instance != null)
        {
            GlobalBrightnessManager.Instance.UpdateBrightness(newValue);
        }
    }
    public void OnMusicChanged(float volumen)
    {
        SetVolume("VolMusic", volumen);
    }

    public void OnFxChanged(float volumen)
    {
        SetVolume("VolFX", volumen);
    }

    private void SetVolume(string parameterName, float normalizedValue)
    {
        if (audioMixer == null) return;

        // Conversión de 0.0001-1 a Decibelios (-80dB a 0dB)
        float dB = Mathf.Log10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    // Convertir de Decibelios (-80dB a 0dB) de vuelta a escala 0-1 para el Slider
    private float DBToNormalized(float dB)
    {
        return Mathf.Pow(10f, dB / 20f);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}