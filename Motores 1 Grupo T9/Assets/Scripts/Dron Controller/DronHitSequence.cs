using System;
using System.Collections;
using UnityEngine;

public class DroneHitSequence : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform cameraPivot;                  // la camara del dron (o el objeto que la sostiene)
    [SerializeField] DroneController drone;
    [SerializeField] CRTGlitchTransitionController glitch;
    [SerializeField] CameraController camController;

    [Header("1. Shake")]
    public bool useShake = true;
    public float shakeDuration = 0.4f;
    public float shakePosIntensity = 0.05f;                  // metros
    public float shakeRotIntensity = 3f;                     // grados
    public AnimationCurve shakeFalloff = AnimationCurve.Linear(0, 1, 1, 0); // 1 = fuerza total

    [Header("2. Caida de camara")]
    public bool useFall = true;
    public float delayBeforeFall = 0.1f;                     // desde que termina el shake
    public float fallDuration = 0.6f;
    public Vector3 fallMaxRotation = new Vector3(25f, 0f, 40f); // rotacion final (grados, local)
    public Vector3 fallOffset = new Vector3(0.1f, -0.15f, 0f);  // cuanto se desplaza (local)
    public bool randomizeSide = false;                       // cae a izquierda o derecha al azar
    public AnimationCurve fallCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 2, 0)); // arranca lento y acelera


    [Header("3. Glitch y minijuego")]
    public float delayAfterFall = 1f;                        // desde que cae hasta que arranca el minijuego
    public bool useGlitch = true;

    public bool IsPlaying { get; private set; }

    Vector3 originPos;
    Quaternion originRot;

    void Awake()
    {
        if (cameraPivot != null)
        {
            originPos = cameraPivot.localPosition;
            originRot = cameraPivot.localRotation;
        }
    }

    public void Play(Action onFinished)
    {
        if (IsPlaying) return;
        StartCoroutine(Sequence(onFinished));
    }

    // Llamar cuando el dron se descongela (TAB)
    public void ResetCamera()
    {
        StopAllCoroutines();
        IsPlaying = false;
        if (camController != null) camController.enabled = true;
        if (cameraPivot == null) return;
        cameraPivot.localPosition = originPos;
        cameraPivot.localRotation = originRot;
    }

    IEnumerator Sequence(Action onFinished)
    {
        IsPlaying = true;
        if (drone != null) drone.FreezeDrone();
        if (camController != null) camController.enabled = false;

        if (useShake && cameraPivot != null)
            yield return Shake();

        if (useFall && cameraPivot != null)
        {
            yield return new WaitForSeconds(delayBeforeFall);
            yield return Fall();
        }

        yield return new WaitForSeconds(delayAfterFall);

        if (useGlitch && glitch != null)
            glitch.TriggerGlitchTransition(() => onFinished?.Invoke()); // el minijuego arranca en el pico del glitch
        else
            onFinished?.Invoke();
        // IsPlaying queda true hasta ResetCamera(), asi no se pisa otra secuencia
    }

    IEnumerator Shake()
    {
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float k = shakeFalloff.Evaluate(t / shakeDuration);

            cameraPivot.localPosition = originPos + UnityEngine.Random.insideUnitSphere * shakePosIntensity * k;
            cameraPivot.localRotation = originRot * Quaternion.Euler(
                UnityEngine.Random.Range(-1f, 1f) * shakeRotIntensity * k,
                UnityEngine.Random.Range(-1f, 1f) * shakeRotIntensity * k,
                UnityEngine.Random.Range(-1f, 1f) * shakeRotIntensity * k);
            yield return null;
        }
        cameraPivot.localPosition = originPos;
        cameraPivot.localRotation = originRot;
    }

    IEnumerator Fall()
    {
        float side = (randomizeSide && UnityEngine.Random.value < 0.5f) ? -1f : 1f;
        Vector3 rot = new Vector3(fallMaxRotation.x, fallMaxRotation.y * side, fallMaxRotation.z * side);
        Vector3 off = new Vector3(fallOffset.x * side, fallOffset.y, fallOffset.z);

        Quaternion endRot = originRot * Quaternion.Euler(rot);
        Vector3 endPos = originPos + off;

        float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            float k = fallCurve.Evaluate(Mathf.Clamp01(t / fallDuration));
            cameraPivot.localPosition = Vector3.LerpUnclamped(originPos, endPos, k);
            cameraPivot.localRotation = Quaternion.SlerpUnclamped(originRot, endRot, k);
            yield return null;
        }
        cameraPivot.localPosition = endPos;
        cameraPivot.localRotation = endRot;
    }
}