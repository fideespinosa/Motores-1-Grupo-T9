using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            ApplyBrightness(BrightnessManager.Instance.Brightness);
        }
    }

    public void ApplyBrightness(float value)
    {
        if (colorAdjustments == null)
            return;

        colorAdjustments.postExposure.value = value;
    }
}