using UnityEngine;

public class PlaceableItem : MonoBehaviour, IInteractable
{
    [Header("Item Id")]
    [Tooltip("Identificador único de este objeto. El PlacementPoint que lo reciba debe usar el mismo id.")]
    [SerializeField] private string itemId;

    [Header("Requirement")]
    [Tooltip("Flag que debe estar activo para poder agarrar este objeto. Dejar vacío si no aplica.")]
    [SerializeField] private string requiredFlag;

    [Header("Placement Point")]
    [Tooltip("GameObject del PlacementPoint correspondiente, que empieza desactivado en la escena y se activa al agarrar este objeto.")]
    [SerializeField] private GameObject placementPointToActivate;

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
        if (PlacementManager.Instance == null) return;

        Debug.Log("Sonido de objeto recogido");

        PlacementManager.Instance.PickUp(itemId);

        if (placementPointToActivate != null)
        {
            placementPointToActivate.SetActive(true);
        }

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