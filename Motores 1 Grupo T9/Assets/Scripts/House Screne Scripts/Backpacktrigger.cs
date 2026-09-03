using System.Collections;
using UnityEngine;

public class BackpackTrigger : MonoBehaviour, IInteractable
{
    [Header("Requirement")]
    [Tooltip("Flag que debe estar activo para que esta secuencia se dispare al mirar el objeto. Dejar vacío si no aplica.")]
    [SerializeField] private string requiredFlag;

    [Header("Sequence")]
    [TextArea]
    [SerializeField] private string triggerText;
    [Tooltip("Cuántos segundos se mantiene la cámara y el movimiento congelados antes de devolver el control.")]
    [SerializeField] private float holdDuration = 2.5f;
    [Tooltip("Flag que se activa al terminar la secuencia (ej. para desbloquear un PlaceableItem).")]
    [SerializeField] private string flagToSetAfter;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float cameraTurnDuration = 0.6f;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool alreadyTriggered = false;

    private bool FlagSatisfied
    {
        get
        {
            if (string.IsNullOrEmpty(requiredFlag)) return true;
            return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag);
        }
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
    }

    public void OnHoverEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }

        if (alreadyTriggered) return;
        if (!FlagSatisfied) return;

        alreadyTriggered = true;
        StartCoroutine(RunSequence());
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private IEnumerator RunSequence()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        yield return StartCoroutine(TurnCameraToTarget());

        if (!string.IsNullOrEmpty(triggerText) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(triggerText);
        }

        yield return new WaitForSeconds(holdDuration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (!string.IsNullOrEmpty(flagToSetAfter) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetAfter);
        }
    }

    private IEnumerator TurnCameraToTarget()
    {
        if (playerCameraTransform == null) yield break;

        Quaternion startRotation = playerCameraTransform.rotation;
        Vector3 direction = (transform.position - playerCameraTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < cameraTurnDuration)
        {
            elapsed += Time.deltaTime;
            playerCameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / cameraTurnDuration);
            yield return null;
        }

        playerCameraTransform.rotation = targetRotation;
    }
}