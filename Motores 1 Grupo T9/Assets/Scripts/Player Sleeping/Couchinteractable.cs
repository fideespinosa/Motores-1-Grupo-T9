using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CouchInteractable : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [Tooltip("Canvas (o GameObject) que muestra el ícono/texto de la tecla E")]
    [SerializeField] private GameObject promptCanvas;

    [Tooltip("Controlador que ejecuta la secuencia de sentarse y dormirse")]
    [SerializeField] private SleepSequenceController sleepSequenceController;

    private bool sequenceActive;

    private void Awake()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    // IInteractable es llamado por Crosshair cuando el jugador clickea mirando el sillon
    public void Action()
    {
        if (sequenceActive || sleepSequenceController == null) return;

        sequenceActive = true;
        SetPromptVisible(false);
        sleepSequenceController.BeginSitSequence();
    }

    // IInteractable llamado por Crosshair cuando el raycast empieza a mirar el sillon
    public void OnHoverEnter()
    {
        SetPromptVisible(true);
    }

    // IInteractable llamado por Crosshair cuando el raycast deja de mirar el sillon
    public void OnHoverExit()
    {
        SetPromptVisible(false);
    }

    private void SetPromptVisible(bool visible)
    {
        if (sequenceActive) return; // no mostrar el prompt si ya esta en la secuencia
        if (promptCanvas != null)
            promptCanvas.SetActive(visible);
    }

    // Llamar esto desde SleepSequenceController cuando termina toda la secuencia
    // (por si se quiere resetear el estado, por ejemplo si el jugador se quiere levantar).
    public void ResetInteraction()
    {
        sequenceActive = false;
    }
}