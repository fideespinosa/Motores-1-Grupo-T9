
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashbackDistortion : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Renderer[] renderers;

    [Header("Shader")]
    [SerializeField] private string propertyName = "_HologramAmount";

    [Header("Blend Range")]
    [Tooltip("Valor que representa la textura original.")]
    [SerializeField, Range(0f, 1f)] private float minAmount = 0f;

    [Tooltip("Valor máximo que puede alcanzar el efecto holográfico.")]
    [SerializeField, Range(0f, 1f)] private float maxAmount = 1f;

    [Header("Distortion Pattern")]
    [Tooltip("Valores entre 0 y 1 relativos al rango Min/Max Amount.")]
    [SerializeField, Range(0f, 1f)]
    private float[] pattern = { 0.8f, 0.5f, 0.8f, 0f };

    [SerializeField, Min(0.1f)]
    private float stepDuration = 1.5f;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float minWait = 8f;
    [SerializeField, Min(0f)] private float maxWait = 20f;

    private const float StepVariation = 0.2f;

    private readonly List<Material> materials = new List<Material>();

    private int propertyId;
    private float currentAmount;
    private Coroutine loopCoroutine;

    private void OnValidate()
    {
        if (maxWait < minWait)
            maxWait = minWait;

        if (maxAmount < minAmount)
            maxAmount = minAmount;
    }

    private void Awake()
    {
        propertyId = Shader.PropertyToID(propertyName);

        Renderer[] targets =
            renderers != null && renderers.Length > 0
            ? renderers
            : GetComponentsInChildren<Renderer>();

        foreach (Renderer targetRenderer in targets)
        {
            if (targetRenderer == null)
                continue;

            foreach (Material material in targetRenderer.materials)
            {
                if (material != null && material.HasProperty(propertyId))
                {
                    materials.Add(material);
                }
            }
        }

        if (materials.Count == 0)
        {
            Debug.LogWarning(
                "FlashbackDistortion en " + gameObject.name +
                ": ningún material tiene la propiedad " + propertyName
            );
        }

        Apply(minAmount);
    }

    private void OnEnable()
    {
        loopCoroutine = StartCoroutine(DistortionLoop());
    }

    private void OnDisable()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        Apply(minAmount);
    }

    private IEnumerator DistortionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minWait, maxWait)
            );

            if (pattern != null)
            {
                foreach (float patternValue in pattern)
                {
                    float target = Mathf.Lerp(
                        minAmount,
                        maxAmount,
                        Mathf.Clamp01(patternValue)
                    );

                    float duration = stepDuration * Random.Range(
                        1f - StepVariation,
                        1f + StepVariation
                    );

                    yield return MoveTo(target, duration);
                }
            }

            yield return MoveTo(minAmount, stepDuration);
        }
    }

    private IEnumerator MoveTo(float target, float duration)
    {
        float start = currentAmount;

        if (Mathf.Approximately(start, target))
            yield break;

        if (duration <= 0f)
        {
            Apply(target);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Clamp01(elapsed / duration)
            );

            Apply(Mathf.Lerp(start, target, t));

            yield return null;
        }

        Apply(target);
    }

    private void Apply(float amount)
    {
        currentAmount = Mathf.Clamp(amount, minAmount, maxAmount);

        foreach (Material material in materials)
        {
            if (material != null)
                material.SetFloat(propertyId, currentAmount);
        }
    }

    private void OnDestroy()
    {
        foreach (Material material in materials)
        {
            if (material != null)
                Destroy(material);
        }
    }
}
