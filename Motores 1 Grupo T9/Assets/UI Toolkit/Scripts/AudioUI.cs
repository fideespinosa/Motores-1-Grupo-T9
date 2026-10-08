using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioUI : MonoBehaviour
{
    private UIDocument uIDocument;
    private Slider musicSlider;
    private Slider fxSlider;
    private Toggle musicToggle;
    private Button btnBackFromOptions;

    [Header("Audio Mixer Configuration")]
    [SerializeField] private AudioMixer audioMixer;

    // Guardamos los últimos valores para restaurarlos al quitar el mute
    private float lastMusicVol = 0.5f;
    private float lastFxVol = 0.5f;
    private float lastMasterVol = 0.5f;
    private bool isMuted = false;

    private void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();

        var root = uIDocument.rootVisualElement;

        musicSlider = root.Q<Slider>("MusicSlider");
        fxSlider = root.Q<Slider>("FXSlider");
        musicToggle = root.Q<Toggle>("MuteToggle");
        btnBackFromOptions = root.Q<Button>("BackButton");

        // Configuración del Slider de Música
        if (musicSlider != null)
        {
            musicSlider.lowValue = 0.0001f;
            musicSlider.highValue = 1f;
            musicSlider.value = lastMusicVol;

            musicSlider.RegisterValueChangedCallback(evt =>
            {
                if (!isMuted)
                {
                    lastMusicVol = evt.newValue;
                    SetVolume("VolMusic", lastMusicVol);
                }
            });
        }

        // Configuración del Slider de SFX/FX
        if (fxSlider != null)
        {
            fxSlider.lowValue = 0.0001f;
            fxSlider.highValue = 1f;
            fxSlider.value = lastFxVol;

            fxSlider.RegisterValueChangedCallback(evt =>
            {
                if (!isMuted)
                {
                    lastFxVol = evt.newValue;
                    SetVolume("VolFX", lastFxVol);
                }
            });
        }

        // Configuración del Toggle de Mute
        if (musicToggle != null)
        {
            musicToggle.RegisterValueChangedCallback(evt =>
            {
                ApplyMute(evt.newValue);
            });
        }
    }
    private void ApplyMute(bool mute)
    {
        isMuted = mute;

        if (isMuted)
        {
            // Silenciar todos los canales en el AudioMixer (-80 dB es silencio total)
            if (audioMixer != null)
            {
                audioMixer.SetFloat("VolMaster", -80f);
            }

            // Desactivar visualmente los sliders
            if (musicSlider != null) musicSlider.SetEnabled(false);
            if (fxSlider != null) fxSlider.SetEnabled(false);
        }
        else
        {
            // Habilitar sliders
            if (musicSlider != null) musicSlider.SetEnabled(true);
            if (fxSlider != null) fxSlider.SetEnabled(true);

            // Restaurar volumen del Master a 0 dB y reaplicar los valores de los sliders
            if (audioMixer != null)
            {
                audioMixer.SetFloat("VolMaster", lastMasterVol);
            }

            float currentMusicVal = musicSlider != null ? musicSlider.value : lastMusicVol;
            float currentFxVal = fxSlider != null ? fxSlider.value : lastFxVol;

            SetVolume("VolMusic", currentMusicVal);
            SetVolume("VolFX", currentFxVal);
        }
    }

    private void SetVolume(string parameterName, float normalizedValue)
    {
        if (audioMixer == null) return;

        // Conversión a escala logarítmica (de 0.0001-1 a dB)
        float dB = Mathf.Log10(Mathf.Clamp(normalizedValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }
}
