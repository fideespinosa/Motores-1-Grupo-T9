using UnityEngine;

public class LockedDrawer : MonoBehaviour, IInteractable
{
    [Header("Required Key")]
    [Tooltip("Debe coincidir con el Item Id del CollectibleItem de la llave correspondiente.")]
    [SerializeField] private string requiredItemId;

    [Header("Locked Message")]
    [TextArea]
    [SerializeField] private string lockedMessage;

    [Header("On Unlock")]
    [SerializeField] private Animator drawerAnimator;
    [SerializeField] private string openTriggerName = "Open";

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool isOpen = false;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Action()
    {
        if (isOpen) return;

        bool hasKey = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItemId);

        if (hasKey)
        {
            Open();
        }
        else if (TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(lockedMessage);
        }
    }

    private void Open()
    {
        isOpen = true;
        Debug.Log("se abrio");
        /*if (drawerAnimator != null)
        {
            //drawerAnimator.SetTrigger(openTriggerName);
        }*/
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
