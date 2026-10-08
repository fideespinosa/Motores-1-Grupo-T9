using UnityEngine;
using UnityEngine.UIElements;

public class BrightnessUI : MonoBehaviour
{
    private UIDocument uiDocument;

    private Slider brightnessSlider;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        brightnessSlider = root.Q<Slider>("brightnessSlider");

        if (brightnessSlider == null) return;

        // Rango del slider: 0 (oscuro) a 1 (brillo normal/máximo)
        brightnessSlider.lowValue = 0f;
        brightnessSlider.highValue = 1f;
        //brightnessSlider.value = 1f;

        float savedBrightness = BrightnessManager.Instance.Brightness;
        brightnessSlider.SetValueWithoutNotify(savedBrightness);

        brightnessSlider.RegisterValueChangedCallback(OnBrightnessChanged);
    }

    private void OnDisable()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.UnregisterValueChangedCallback(OnBrightnessChanged);
        }
    }

    private void OnBrightnessChanged(ChangeEvent<float> evt)
    {
        BrightnessManager.Instance.SetBrightness(evt.newValue);
    }
}