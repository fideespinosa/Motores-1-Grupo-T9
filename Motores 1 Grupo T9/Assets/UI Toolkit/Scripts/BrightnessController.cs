using UnityEngine;
using UnityEngine.UIElements;

public class BrightnessController : MonoBehaviour
{
    public static BrightnessController Instance { get; private set; }

    private UIDocument uiDocument;
    [SerializeField] private string overlayElementName = "brightnessOverlay";

    private VisualElement overlayElement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        overlayElement = root.Q<VisualElement>(overlayElementName);

        root.pickingMode = PickingMode.Ignore;
        overlayElement.pickingMode = PickingMode.Ignore;

        if (BrightnessManager.Instance != null)
        {
            ApplyBrightness(BrightnessManager.Instance.Brightness);
        }
    }

    public void ApplyBrightness(float value)
    {
        if (overlayElement == null) return;

        // value = 1 -> alpha = 0.0 (transparente, claro)
        // value = 0 -> alpha = 0.8 (pantalla oscura)
        float maxDarkness = 0.8f; // Límite para no dejar la pantalla 100% negra
        float alpha = (1f - Mathf.Clamp01(value)) * maxDarkness;

        overlayElement.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, alpha));
    }
}