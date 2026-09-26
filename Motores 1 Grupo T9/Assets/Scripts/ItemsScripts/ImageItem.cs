using UnityEngine;

public class ImageItem : MonoBehaviour, IInteractable
{
    [Header("Image To Show")]
    [SerializeField] private Sprite imageToShow;

    [Header("On Interact")]
    [Tooltip("Si está activo, el objeto desaparece de la escena después de mostrarse una vez.")]
    [SerializeField] private bool disableOnInteract = false;

    [Header("Requirement")]
    [Tooltip("Flag que debe estar activo para poder interactuar con este objeto. Dejar vacío si no aplica.")]
    [SerializeField] private string requiredFlag;

    [Header("Outline")]
    [Tooltip("Script de outline del objeto. Se activa al mirarlo y se desactiva al dejar de mirarlo.")]
    [SerializeField] private Outline outline;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip paperOn;
    [SerializeField] private AudioClip paperOff;

    private bool IsUnlocked()
    {
        if (string.IsNullOrEmpty(requiredFlag)) return true;
        return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag);
    }

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Action()
    {
        if (!IsUnlocked()) return;

        if (ImagePanelManager.Instance != null)
        {
            ImagePanelManager.Instance.ShowImage(imageToShow);
            audioSource.PlayOneShot(paperOn);

        }
        else
        {
            Debug.LogWarning("No se encontró un ImagePanelManager en la escena");
        }

        if (disableOnInteract)
        {
            gameObject.SetActive(false);
            audioSource.PlayOneShot(paperOff);
        }
    }

    public void OnHoverEnter()
    {
        if (!IsUnlocked()) return;

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