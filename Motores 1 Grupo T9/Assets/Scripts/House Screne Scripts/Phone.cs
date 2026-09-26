using System.Collections;
using UnityEngine;

public class Phone : MonoBehaviour, IInteractable
{
    private enum PhoneState
    {
        Idle,
        Ringing,
        Answering,
        Finished
    }

    [Header("Call")]
    [Tooltip("Duración en segundos del mensaje grabado. La cámara y el movimiento quedan congelados hasta que termine.")]
    [SerializeField] private float messageDuration = 5f;

    [Header("Final Dialogue")]
    [TextArea]
    [SerializeField] private string finalDialogueText;

    [Header("Story Flag")]
    [SerializeField] private string flagToSetOnFinish;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float cameraTurnDuration = 0.6f;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip phoneRing;
    [SerializeField] private AudioClip debtCall;


    private PhoneState state = PhoneState.Idle;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
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
        audioSource.PlayOneShot(phoneRing);
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

        Debug.Log("Reproduciendo grabación...");

        audioSource.Stop();

        audioSource.clip = debtCall;

        audioSource.Play();

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

        state = PhoneState.Finished;

        if (!string.IsNullOrEmpty(finalDialogueText) && TextPanelManager.Instance != null)
        {
            TextPanelManager.Instance.ShowText(finalDialogueText);
        }

        if (!string.IsNullOrEmpty(flagToSetOnFinish) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetOnFinish);
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