using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    [Header("Item Id")]
    [Tooltip("Identificador único de este item, ej. 'red_backpack_key'. Se usa para chequear el inventario después.")]
    [SerializeField] private string itemId;

    [Header("On Pickup")]
    [Tooltip("Texto opcional que dice el protagonista al agarrarlo. Dejalo vacío si no querés comentario.")]
    [TextArea]
    [SerializeField] private string pickupComment;
    [SerializeField] private bool disableOnPickup = true;
    [Tooltip("GameObjects que se activan al agarrar este item (ej. un trigger de cinemática).")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Header("Requirement")]
    [SerializeField] private string requiredFlag;

    [Header("Story Flag")]
    [SerializeField] private string flagToSetOnPickup;

    [Header("Outline")]
    [SerializeField] private Outline outline;

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

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemId);
        }

        if (!string.IsNullOrEmpty(pickupComment) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(pickupComment);
        }

        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        if (!string.IsNullOrEmpty(flagToSetOnPickup) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetOnPickup);
        }

        if (disableOnPickup)
        {
            gameObject.SetActive(false);
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