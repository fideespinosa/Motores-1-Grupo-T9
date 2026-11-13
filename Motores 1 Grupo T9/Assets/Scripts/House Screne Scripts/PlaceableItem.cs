
using UnityEngine;
using UnityEngine.Events;

public class PlaceableItem : MonoBehaviour, IInteractable, IInteractionGate
{
    [Header("Item Id")]
    [SerializeField] private string itemId;

    [Header("Requirement")]
    [SerializeField] private string requiredFlag;

    [Header("Placement Point")]
    [SerializeField] private GameObject placementPointToActivate;

    [Header("Story Flag")]
    [SerializeField] private string flagToSetOnPickup;

    [Header("On Pickup Event")]
    [SerializeField] private UnityEvent onPickupEvent;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveObj;
    [SerializeField] private AudioClip pickObj;

    private bool IsUnlocked()
    {
        if (string.IsNullOrEmpty(requiredFlag)) return true;
        return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag);
    }

    public bool CanInteract()
    {
        return IsUnlocked();
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
        if (PlacementManager.Instance == null) return;

        if (audioSource != null && pickObj != null)
            audioSource.PlayOneShot(pickObj);

        PlacementManager.Instance.PickUp(itemId);

        if (placementPointToActivate != null)
        {
            placementPointToActivate.SetActive(true);
        }

        if (!string.IsNullOrEmpty(flagToSetOnPickup) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetOnPickup);

            if (audioSource != null && moveObj != null)
            {
                audioSource.Stop();
                audioSource.clip = moveObj;
                audioSource.Play();
            }
        }

        onPickupEvent?.Invoke();

        gameObject.SetActive(false);
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
