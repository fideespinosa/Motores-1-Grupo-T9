using UnityEngine;
using UnityEngine.UIElements;

public class BrightnessUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private BrightnessController brightnessController;

    private Slider brightnessSlider;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;

        brightnessSlider = root.Q<Slider>("brightnessSlider");

        brightnessSlider.lowValue = -2f;
        brightnessSlider.highValue = 2f;

        brightnessSlider.SetValueWithoutNotify(
        BrightnessManager.Instance.Brightness
        );

        brightnessController.ApplyBrightness(
        BrightnessManager.Instance.Brightness
        );

        brightnessSlider.RegisterValueChangedCallback(OnBrightnessChanged);
    }

    private void OnBrightnessChanged(ChangeEvent<float> evt)
    {
        BrightnessManager.Instance.SetBrightness(evt.newValue);

        brightnessController.ApplyBrightness(evt.newValue);
    }
}