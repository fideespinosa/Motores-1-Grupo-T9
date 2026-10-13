using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorLookEvent : MonoBehaviour
{
    [SerializeField] private CinematicManager cinematicManager;

    [Header("Light")]
    [SerializeField] private List<Light> pointLights;
    [SerializeField] private List<Light> spotLights;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float targetIntensity = 5f;

    [Header("Blind Effect")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup blindCanvasGroup;
    [SerializeField] private float blindFadeInDuration = 0.2f;
    [SerializeField] private float blindHoldDuration = 0.3f;
    [SerializeField] private float blindFadeOutDuration = 2f;
    [SerializeField, Range(0f, 1f)] private float maxBlindAlpha = 0.5f;

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
    private Coroutine blindCoroutine;

    public void StartLookSequence()
    {
        if (isRunning) return;
        if (lookTarget == null || playerCameraTransform == null) return;

        StartCoroutine(RunSequence());
    }

    public void LightOn()
    {
        foreach (Light pointLight in pointLights)
        {
            if (pointLight == null) continue;

            pointLight.intensity = 0f;
            pointLight.gameObject.SetActive(true);
            StartCoroutine(FadeInLight(pointLight, targetIntensity));
        }
    }

    public void SpotLightOn()
    {
        foreach (Light spotLight in spotLights)
        {
            if (spotLight == null) continue;

            spotLight.intensity = 0f;
            spotLight.gameObject.SetActive(true);
            StartCoroutine(FadeInLight(spotLight, 100f));
        }
    }

    private IEnumerator FadeInLight(Light targetLight, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            targetLight.intensity = Mathf.Lerp(0f, intensity, elapsed / duration);
            yield return null;
        }

        targetLight.intensity = intensity;
    }

    public void BlindEffect()
    {
        if (panel == null || blindCanvasGroup == null) return;

        if (blindCoroutine != null)
        {
            StopCoroutine(blindCoroutine);
        }

        blindCoroutine = StartCoroutine(BlindCoroutine());
    }

    private IEnumerator BlindCoroutine()
    {
        panel.SetActive(true);
        blindCanvasGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < blindFadeInDuration)
        {
            elapsed += Time.deltaTime;

            blindCanvasGroup.alpha = Mathf.Lerp(
                0f,
                maxBlindAlpha,
                elapsed / blindFadeInDuration
            );

            yield return null;
        }

        blindCanvasGroup.alpha = maxBlindAlpha;

        yield return new WaitForSeconds(blindHoldDuration);

        elapsed = 0f;

        while (elapsed < blindFadeOutDuration)
        {
            elapsed += Time.deltaTime;

            blindCanvasGroup.alpha = Mathf.Lerp(
                maxBlindAlpha,
                0f,
                elapsed / blindFadeOutDuration
            );

            yield return null;
        }

        blindCanvasGroup.alpha = 0f;
        panel.SetActive(false);
        blindCoroutine = null;
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

        Quaternion targetRotation = direction.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(direction.normalized)
            : startRotation;

        yield return RotateCamera(startRotation, targetRotation, turnDuration);
        yield return new WaitForSeconds(holdDuration);

        if (playerMovement != null && movementWasEnabled)
        {
            playerMovement.enabled = true;
        }

        if (canvasWasActive)
        {
            canvasObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(flagToSetAfter) &&
            StoryFlagManager.Instance != null)
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

            float t = duration > 0f
                ? Mathf.SmoothStep(0f, 1f, elapsed / duration)
                : 1f;

            playerCameraTransform.rotation = Quaternion.Slerp(from, to, t);

            yield return null;
        }

        playerCameraTransform.rotation = to;
    }

    public void moveBackwards()
    {
        if (cinematicManager != null)
        {
            cinematicManager.StartKnockback();
        }
    }
}