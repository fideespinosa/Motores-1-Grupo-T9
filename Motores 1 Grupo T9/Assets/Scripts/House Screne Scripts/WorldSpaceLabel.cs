using TMPro;
using UnityEngine;

public class WorldSpaceLabel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI labelText;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Show(string text)
    {
        if (canvasGroup == null || labelText == null)
        {
            Debug.LogWarning("WorldSpaceLabel en " + gameObject.name + " tiene referencias sin asignar (canvasGroup o labelText).", this);
            return;
        }

        labelText.text = text;
        canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
    }
}