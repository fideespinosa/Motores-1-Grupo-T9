
using UnityEngine;
using UnityEngine.Events;

public class PlacementPoint : MonoBehaviour, IInteractable
{
    [Header("Accepted Item")]
    [SerializeField] private string acceptedItemId;

    [Header("On Placement")]
    [SerializeField] private GameObject placedVisual;

    [Header("Wrong Item Message")]
    [TextArea]
    [SerializeField] private string wrongItemMessage;

    [Header("On Success")]
    [TextArea]
    [SerializeField] private string successMessage;
    [SerializeField] private string flagToSet;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private bool disableOnSuccess = true;

    [Header("On Interact Event")]
    [SerializeField] private UnityEvent onInteractSuccess;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip waterClip;

    private bool isFilled = false;

    private void Awake()
    {
        if (outline != null)
            outline.enabled = false;

        if (placedVisual != null)
            placedVisual.SetActive(false);
    }

    public void Action()
    {
        if (isFilled) return;
        if (PlacementManager.Instance == null || !PlacementManager.Instance.HasHeldItem) return;

        bool success = PlacementManager.Instance.TryConsume(acceptedItemId);

        if (success)
        {
            isFilled = true;

            if (audioSource != null && waterClip != null)
                audioSource.PlayOneShot(waterClip);

            if (placedVisual != null)
                placedVisual.SetActive(true);

            if (outline != null)
                outline.enabled = false;

            if (!string.IsNullOrEmpty(flagToSet) && StoryFlagManager.Instance != null)
                StoryFlagManager.Instance.SetFlag(flagToSet);

            if (objectsToActivate != null)
            {
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null)
                        obj.SetActive(true);
                }
            }

            if (!string.IsNullOrEmpty(successMessage) && TextPanelManager.Instance != null)
                TextPanelManager.Instance.ShowText(successMessage);

            onInteractSuccess?.Invoke();

            if (disableOnSuccess)
                gameObject.SetActive(false);
        }
        else
        {
            if (!string.IsNullOrEmpty(wrongItemMessage) && TextPanelManager.Instance != null)
                TextPanelManager.Instance.ShowText(wrongItemMessage);
        }
    }

    public void OnHoverEnter()
    {
        if (isFilled) return;

        if (outline != null)
            outline.enabled = true;
    }

    public void OnHoverExit()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
