using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SleepSequenceController : MonoBehaviour
{
    [Header("Referencias - Jugador / Cámara")]
    [Tooltip("Transform de la cámara del jugador (o del rig de cámara)")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("Punto de destino donde la cámara queda 'sentada' mirando la radio")]
    [SerializeField] private Transform sitPoint;

    [Tooltip("Script de movimiento/mouse look del jugador, se desactiva mientras está sentado")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    [Tooltip("Script de interacción del jugador (opcional), se desactiva mientras está sentado")]
    [SerializeField] private MonoBehaviour playerInteractionScript;

    [Tooltip("Renderers de la malla/modelo del jugador (MeshRenderer o SkinnedMeshRenderer), se desactivan mientras está sentado. NO uses el GameObject completo si la cámara cuelga del mismo objeto.")]
    [SerializeField] private Renderer[] playerRenderers;

    [Header("Referencias - Sentado")]
    [SerializeField] private CouchInteractable couchInteractable;

    [Header("Audio - Radio")]
    [SerializeField] private AudioSource radioAudioSource;
    [SerializeField] private float radioStartVolume = 1f;
    [SerializeField] private float radioSleepVolume = 0.15f;

    [Header("Post Processing (Blur)")]
    [Tooltip("Volume de URP/HDRP con el perfil de blur (Depth of Field / Vignette) en weight 0")]
    [SerializeField] private Volume sleepPostProcessVolume;

    [Tooltip("Fracción (0-1) de blur alcanzada en la Fase 1, antes de los parpadeos")]
    [Range(0.1f, 1f)]
    [SerializeField] private float midBlurWeight = 0.4f;

    [Header("Canvas - Párpados")]
    [SerializeField] private CanvasGroup sleepCanvasGroup;
    [Tooltip("Rect del párpado superior")]
    [SerializeField] private RectTransform topEyelid;
    [Tooltip("Rect del párpado inferior")]
    [SerializeField] private RectTransform bottomEyelid;
    [Tooltip("Posición Y (anchoredPosition) de cada párpado cuando está totalmente abierto")]
    [SerializeField] private float topEyelidOpenY = 400f;
    [SerializeField] private float bottomEyelidOpenY = -400f;
    [Tooltip("Posición Y de cada párpado cuando está totalmente cerrado (se tocan en el centro)")]
    [SerializeField] private float eyelidsClosedY = 0f;

    [Header("Timing")]
    [Tooltip("Duración del movimiento de la cámara hasta el punto de sentado")]
    [SerializeField] private float sitTransitionDuration = 1.2f;

    [Tooltip("Tiempo que el personaje escucha la radio antes de empezar a dormirse")]
    [SerializeField] private float timeBeforeFallingAsleep = 15f;

    [Header("Timing - Fases de dormirse")]
    [Tooltip("Fase 1: duración de la subida inicial de blur, apenas termina de sentarse")]
    [SerializeField] private float blurRampDuration = 2f;

    [Tooltip("Fase 2: cantidad de parpadeos antes de dormirse del todo")]
    [SerializeField] private int blinkCount = 3;

    [Tooltip("Fase 2: duración de cada cierre de ojos en un parpadeo")]
    [SerializeField] private float blinkCloseDuration = 0.3f;

    [Tooltip("Fase 2: duración de cada apertura de ojos en un parpadeo")]
    [SerializeField] private float blinkOpenDuration = 0.5f;

    [Tooltip("Fase 3: duración de la subida de blur desde Mid Blur Weight hasta el máximo")]
    [SerializeField] private float additionalBlurDuration = 2.5f;

    [Tooltip("Fase 4: duración del cierre final de párpados hasta la oscuridad total")]
    [SerializeField] private float finalEyelidsCloseDuration = 1.5f;

    [Tooltip("Fase 5: duración de la baja de volumen de la radio, ya en oscuridad")]
    [SerializeField] private float volumeFadeDuration = 3f;

    [Tooltip("Curva de easing usada en todas las transiciones")]
    [SerializeField] private AnimationCurve transitionEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine sequenceRoutine;

    public void BeginSitSequence()
    {
        if (sequenceRoutine != null) return;
        sequenceRoutine = StartCoroutine(SitSequenceRoutine());
    }

    private IEnumerator SitSequenceRoutine()
    {
        SetPlayerControlEnabled(false);

        if (playerRenderers != null)
        {
            foreach (Renderer r in playerRenderers)
            {
                if (r != null) r.enabled = false;
            }
        }
        // Pasos del sistema

        // 1) Mover cámara hacia el punto de sentado
        Vector3 startPos = playerCamera.position;
        Quaternion startRot = playerCamera.rotation;
        float t = 0f;

        while (t < sitTransitionDuration)
        {
            t += Time.deltaTime;
            float lerpT = transitionEase.Evaluate(Mathf.Clamp01(t / sitTransitionDuration));
            playerCamera.position = Vector3.Lerp(startPos, sitPoint.position, lerpT);
            playerCamera.rotation = Quaternion.Slerp(startRot, sitPoint.rotation, lerpT);
            yield return null;
        }

        playerCamera.position = sitPoint.position;
        playerCamera.rotation = sitPoint.rotation;

        // 2) Prender la radio
        if (radioAudioSource != null)
        {
            radioAudioSource.volume = radioStartVolume;
            if (!radioAudioSource.isPlaying)
                radioAudioSource.Play();
        }

        // 3) Esperar el tiempo antes de empezar a dormirse
        yield return new WaitForSeconds(timeBeforeFallingAsleep);

        yield return StartCoroutine(FallAsleepRoutine());
    }

    private IEnumerator FallAsleepRoutine()
    {
        if (sleepCanvasGroup != null)
            sleepCanvasGroup.alpha = 1f;

        SetEyelidsPosition(0f); // arranca con los ojos abiertos

        // Fase 1: empieza el blur
        yield return StartCoroutine(LerpBlur(0f, midBlurWeight, blurRampDuration));

        // Fase 2: parpadeos
        for (int i = 0; i < blinkCount; i++)
        {
            yield return StartCoroutine(LerpEyelids(0f, 1f, blinkCloseDuration));
            yield return StartCoroutine(LerpEyelids(1f, 0f, blinkOpenDuration));
        }

        // Fase 3: mas blur
        yield return StartCoroutine(LerpBlur(midBlurWeight, 1f, additionalBlurDuration));

        // Fase 4:los parpados se cierran del todo y se quedan cerrados
        yield return StartCoroutine(LerpEyelids(0f, 1f, finalEyelidsCloseDuration));

        // Fase 5: baja el volumen de la radio
        float currentVolume = radioAudioSource != null ? radioAudioSource.volume : radioStartVolume;
        yield return StartCoroutine(LerpVolume(currentVolume, radioSleepVolume, volumeFadeDuration));

        OnFellAsleep();
    }

    private IEnumerator LerpBlur(float fromWeight, float toWeight, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpT = transitionEase.Evaluate(Mathf.Clamp01(elapsed / duration));
            if (sleepPostProcessVolume != null)
                sleepPostProcessVolume.weight = Mathf.Lerp(fromWeight, toWeight, lerpT);
            yield return null;
        }
        if (sleepPostProcessVolume != null)
            sleepPostProcessVolume.weight = toWeight;
    }

    private IEnumerator LerpEyelids(float fromT, float toT, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpT = transitionEase.Evaluate(Mathf.Clamp01(elapsed / duration));
            SetEyelidsPosition(Mathf.Lerp(fromT, toT, lerpT));
            yield return null;
        }
        SetEyelidsPosition(toT);
    }

    private IEnumerator LerpVolume(float fromVolume, float toVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpT = transitionEase.Evaluate(Mathf.Clamp01(elapsed / duration));
            if (radioAudioSource != null)
                radioAudioSource.volume = Mathf.Lerp(fromVolume, toVolume, lerpT);
            yield return null;
        }
        if (radioAudioSource != null)
            radioAudioSource.volume = toVolume;
    }

    // t: 0 = ojos totalmente abiertos, 1 = ojos totalmente cerrados
    private void SetEyelidsPosition(float t)
    {
        if (topEyelid != null)
        {
            Vector2 pos = topEyelid.anchoredPosition;
            pos.y = Mathf.Lerp(topEyelidOpenY, eyelidsClosedY, t);
            topEyelid.anchoredPosition = pos;
        }

        if (bottomEyelid != null)
        {
            Vector2 pos = bottomEyelid.anchoredPosition;
            pos.y = Mathf.Lerp(bottomEyelidOpenY, eyelidsClosedY, t);
            bottomEyelid.anchoredPosition = pos;
        }
    }

    private void OnFellAsleep()
    {
        // Punto de extension: para fate a negro
        // cargar otra escena, (el arranque de la segunda parte)
        sequenceRoutine = null;
    }

    private void SetPlayerControlEnabled(bool enabled)
    {
        if (playerMovementScript != null) playerMovementScript.enabled = enabled;
        if (playerInteractionScript != null) playerInteractionScript.enabled = enabled;
    }
}