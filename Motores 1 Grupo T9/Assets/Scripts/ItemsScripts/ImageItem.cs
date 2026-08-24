using UnityEngine;

public class ImageItem : MonoBehaviour, IInteractable
{
    [Header("Imagen")]
    [SerializeField] private Sprite imageToShow;
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
        if (ImagePanelManager.Instance != null)
        {
            ImagePanelManager.Instance.ShowImage(imageToShow);
        }
        else
        {
            Debug.LogWarning("no se encuentra ImagePanelManager en la escena");
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