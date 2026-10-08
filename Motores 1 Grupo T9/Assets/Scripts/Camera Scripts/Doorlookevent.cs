using System.Collections;
using UnityEngine;

public class DoorLookEvent : MonoBehaviour
{
    [Header("Look Target")]
    [SerializeField] private Transform lookTarget;

    [Header("Timing")]
    [SerializeField] private float turnDuration = 0.6f;
    [SerializeField] private float holdDuration = 2.5f;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform playerCameraTransform;

    [Header("Canvas")]
    [SerializeField] private GameObject canvasObject;

    [Header("Story Flag")]
    [SerializeField] private string flagToSetAfter;

    private bool isRunning = false;

    public void StartLookSequence()
    {
        if (isRunning) return;
        if (lookTarget == null || playerCameraTransform == null) return;

        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        isRunning = true;

        bool movementWasEnabled = playerMovement != null && playerMovement.enabled;
        bool canvasWasActive = canvasObject != null && canvasObject.activeSelf;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (canvasWasActive)
        {
            canvasObject.SetActive(false);
        }

        Quaternion startRotation = playerCameraTransform.rotation;
        Vector3 direction = lookTarget.position - playerCameraTransform.position;
        Quaternion targetRotation = direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction.normalized) : startRotation;

        yield return RotateCamera(startRotation, targetRotation, turnDuration);
        yield return new WaitForSeconds(holdDuration);

        if (playerMovement != null)
        {
            playerMovement.SetLookDirection(direction);

            if (movementWasEnabled)
            {
                playerMovement.enabled = true;
            }
        }

        if (canvasWasActive)
        {
            canvasObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(flagToSetAfter) && StoryFlagManager.Instance != null)
        {
            StoryFlagManager.Instance.SetFlag(flagToSetAfter);
        }

        isRunning = false;
    }

    private IEnumerator RotateCamera(Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            playerCameraTransform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        playerCameraTransform.rotation = to;
    }
}