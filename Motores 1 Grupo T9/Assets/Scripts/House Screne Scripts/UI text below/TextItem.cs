using UnityEngine;

public class TextItem : MonoBehaviour, IInteractable
{
    [Header("Comment To Show")]
    [TextArea]
    [SerializeField] private string commentText;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Action()
    {
        if (TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(commentText);
        }
        else
        {
            Debug.LogWarning("No se encontró un TextPanelManager en la escena.");
        }
    }

    public void OnHoverEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}