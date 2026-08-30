using System.Collections;
using UnityEngine;

public class Phone : MonoBehaviour, IInteractable
{
    private enum PhoneState
    {
        Idle,
        Ringing,
        Answering,
        WaitingToMoveAway,
        Finished
    }

    [Header("First Call")]
    [Tooltip("Duración en segundos del primer mensaje grabado. La cámara y el movimiento quedan congelados hasta que termine.")]
    [SerializeField] private float firstMessageDuration = 5f;

    [Header("Second Call")]
    [Tooltip("Duración en segundos del segundo mensaje grabado.")]
    [SerializeField] private float secondMessageDuration = 5f;
    [Tooltip("Qué tan lejos tiene que alejarse el jugador del teléfono después de la primera llamada para que suene de nuevo.")]
    [SerializeField] private float moveAwayDistance = 4f;

    [Header("Final Dialogue")]
    [TextArea]
    [SerializeField] private string finalDialogueText;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float cameraTurnDuration = 0.6f;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private PhoneState state = PhoneState.Idle;
    private bool isSecondCall = false;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void Update()
    {
        if (state != PhoneState.WaitingToMoveAway) return;
        if (playerMovement == null) return;

        float distance = Vector3.Distance(playerMovement.transform.position, transform.position);
        if (distance >= moveAwayDistance)
        {
            StartRinging();
        }
    }

    public void StartRingingSequence()
    {
        if (state != PhoneState.Idle) return;
        StartRinging();
    }

    private void StartRinging()
    {
        state = PhoneState.Ringing;
        Debug.Log("Sonando...");
    }

    public void Action()
    {
        if (state != PhoneState.Ringing) return;

        state = PhoneState.Answering;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        StartCoroutine(TurnCameraToPhone());

        float messageDuration = isSecondCall ? secondMessageDuration : firstMessageDuration;
        Debug.Log(isSecondCall ? "Reproduciendo segunda grabación..." : "Reproduciendo primera grabación...");

        StartCoroutine(WaitForMessageEnd(messageDuration));
    }

    private IEnumerator TurnCameraToPhone()
    {
        if (playerCameraTransform == null) yield break;

        Quaternion startRotation = playerCameraTransform.rotation;
        Vector3 directionToPhone = (transform.position - playerCameraTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPhone);

        float elapsed = 0f;
        while (elapsed < cameraTurnDuration)
        {
            elapsed += Time.deltaTime;
            playerCameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / cameraTurnDuration);
            yield return null;
        }

        playerCameraTransform.rotation = targetRotation;
    }

    private IEnumerator WaitForMessageEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndCall();
    }

    private void EndCall()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (!isSecondCall)
        {
            isSecondCall = true;
            state = PhoneState.WaitingToMoveAway;
        }
        else
        {
            state = PhoneState.Finished;

            if (!string.IsNullOrEmpty(finalDialogueText) && TextPanelManager.Instance != null)
            {
                TextPanelManager.Instance.ShowText(finalDialogueText);
            }
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