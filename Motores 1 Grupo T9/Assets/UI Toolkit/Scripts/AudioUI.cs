using UnityEngine;
using UnityEngine.UIElements;

public class AudioUI : MonoBehaviour
{
    private UIDocument uIDocument;
    private Slider musicSlider;
    private Slider fxSlider;
    private Slider brilloSlider;
    private Toggle musicToggle;

    private void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();

        var root = uIDocument.rootVisualElement;

        musicSlider = root.Q<Slider>("MusicSlider");
        fxSlider = root.Q<Slider>("FXSlider");
        brilloSlider = root.Q<Slider>("BrilloSlider");
        musicToggle = root.Q<Toggle>("MuteToggle");
    }
}
