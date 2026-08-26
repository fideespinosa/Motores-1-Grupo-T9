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
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemId);
        }

        if (!string.IsNullOrEmpty(pickupComment) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(pickupComment);
        }

        if (disableOnPickup)
        {
            gameObject.SetActive(false);
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
