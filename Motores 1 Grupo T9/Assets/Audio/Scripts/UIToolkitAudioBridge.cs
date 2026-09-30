using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIToolkitAudioBridge : MonoBehaviour
{
    private UIDocument uiDocument;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            var buttons = uiDocument.rootVisualElement.Query<Button>().ToList();

            foreach (var button in buttons)
            {
                button.clicked += OnButtonClicked;
                button.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            }
        }
    }

    private void OnDisable()
    {
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            var buttons = uiDocument.rootVisualElement.Query<Button>().ToList();

            foreach (var button in buttons)
            {
                button.clicked -= OnButtonClicked;
                button.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            }
        }
    }

    private void OnButtonClicked()
    {
        if (UIMenuAudioManager.Instance != null)
        {
            UIMenuAudioManager.Instance.PlayClick();
        }
    }

    private void OnPointerEnter(PointerEnterEvent evt)
    {
    }
}