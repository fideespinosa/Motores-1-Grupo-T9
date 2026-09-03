using UnityEngine;

public class PlacementPoint : MonoBehaviour, IInteractable
{
    [Header("Accepted Item")]
    [Tooltip("Debe coincidir con el Item Id del PlaceableItem esperado aquí. Dejar vacío para aceptar cualquier objeto que el jugador tenga en mano.")]
    [SerializeField] private string acceptedItemId;

    [Header("On Placement")]
    [Tooltip("GameObject ya posicionado en el lugar correcto, que se activa al colocar el objeto.")]
    [SerializeField] private GameObject placedVisual;

    [Header("Wrong Item Message")]
    [Tooltip("Texto opcional que se muestra si el jugador intenta colocar algo que no corresponde acá.")]
    [TextArea]
    [SerializeField] private string wrongItemMessage;

    [Header("On Success")]
    [Tooltip("Texto opcional que se muestra al colocar el objeto correctamente.")]
    [TextArea]
    [SerializeField] private string successMessage;
    [Tooltip("Flag que se activa al colocar el objeto correctamente. Dejar vacío si no aplica.")]
    [SerializeField] private string flagToSet;
    [Tooltip("Otros GameObjects que se activan al colocar el objeto correctamente (ej. el trigger del teléfono).")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool isFilled = false;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }

        if (placedVisual != null)
        {
            placedVisual.SetActive(false);
        }
    }

    public void Action()
    {
        if (isFilled) return;
        if (PlacementManager.Instance == null || !PlacementManager.Instance.HasHeldItem) return;

        bool success = PlacementManager.Instance.TryConsume(acceptedItemId);

        if (success)
        {
            isFilled = true;

            Debug.Log("Sonido de objeto colocado");

            if (placedVisual != null)
            {
                placedVisual.SetActive(true);
            }

            if (outline != null)
            {
                outline.enabled = false;
            }

            if (!string.IsNullOrEmpty(flagToSet) && StoryFlagManager.Instance != null)
            {
                StoryFlagManager.Instance.SetFlag(flagToSet);
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

            if (!string.IsNullOrEmpty(successMessage) && TextPanelManager.Instance != null)
            {
                TextPanelManager.Instance.ShowText(successMessage);
            }
        }
        else if (!string.IsNullOrEmpty(wrongItemMessage) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(wrongItemMessage);
        }
    }

    public void OnHoverEnter()
    {
        if (isFilled) return;

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