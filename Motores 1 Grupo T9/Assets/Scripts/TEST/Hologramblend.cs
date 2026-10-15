using System.Collections.Generic;
using UnityEngine;

public class HologramBlend : MonoBehaviour
{
    public enum HologramMode
    {
        Static,
        Alternating,
        Flicker,
        Organic
    }

    private enum CyclePhase
    {
        OriginalHold,
        ToHologram,
        HologramHold,
        ToOriginal
    }

    [Header("Targets")]
    [SerializeField] private Renderer[] renderers;

    [Header("Shader")]
    [SerializeField] private string propertyName = "_HologramAmount";

    [Header("Blend Range")]
    [SerializeField, Range(0f, 1f)]
    private float minAmount = 0f;

    [SerializeField, Range(0f, 1f)]
    private float maxAmount = 1f;

    [SerializeField, Range(0f, 1f)]
    private float startAmount = 0f;

    [Header("Behavior")]
    [SerializeField] private HologramMode mode = HologramMode.Organic;
    [SerializeField] private bool playOnStart = true;

    [Header("Transition")]
    [Tooltip("Tiempo de suavizado. Una transición completa tarda varias veces este valor.")]
    [SerializeField, Min(0f)]
    private float transitionDuration = 0.5f;

    [Header("Alternating Mode")]
    [SerializeField, Min(0f)]
    private float originalHoldTime = 2f;

    [SerializeField, Min(0f)]
    private float hologramHoldTime = 2f;

    [Header("Flicker Mode")]
    [SerializeField, Min(0.01f)]
    private float flickerInterval = 0.1f;

    [SerializeField, Min(0f)]
    private float flickerSmoothTime = 0.02f;

    [Header("Organic Mode")]
    [SerializeField, Min(0.01f)]
    private float changeSpeed = 0.3f;

    [SerializeField, Range(0f, 1f)]
    private float noiseContrast = 1f;

    private readonly List<Material> materials = new List<Material>();

    private int propertyId;
    private float noiseOffset;
    private float currentAmount;
    private float velocity;
    private float manualTarget;
    private float flickerTimer;
    private float flickerTarget;
    private float cycleTimer;
    private float lastApplied = -1f;
    private HologramMode startMode;

    private CyclePhase cyclePhase = CyclePhase.OriginalHold;

    private void OnValidate()
    {
        if (maxAmount < minAmount)
            maxAmount = minAmount;
    }

    private void Awake()
    {
        startMode = mode;
        propertyId = Shader.PropertyToID(propertyName);
        noiseOffset = Random.Range(0f, 1000f);

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
                "HologramBlend en " + gameObject.name +
                ": ningún material tiene la propiedad " + propertyName
            );
        }

        currentAmount = Mathf.Clamp01(startAmount);
        manualTarget = currentAmount;
        flickerTarget = currentAmount;

        Apply(currentAmount);
    }

    private void Start()
    {
        if (!playOnStart)
        {
            mode = HologramMode.Static;
            return;
        }

        ResetBehavior();
    }

    private void Update()
    {
        float target = manualTarget;

        switch (mode)
        {
            case HologramMode.Static:
                target = manualTarget;
                break;

            case HologramMode.Alternating:
                target = UpdateAlternating();
                break;

            case HologramMode.Flicker:
                target = UpdateFlicker();
                break;

            case HologramMode.Organic:
                target = UpdateOrganic();
                break;
        }

        target = Mathf.Clamp(target, minAmount, maxAmount);

        float smoothTime = mode == HologramMode.Flicker ? flickerSmoothTime : transitionDuration;

        if (smoothTime <= 0f)
        {
            currentAmount = target;
            velocity = 0f;
        }
        else
        {
            currentAmount = Mathf.SmoothDamp(
                currentAmount,
                target,
                ref velocity,
                smoothTime
            );
        }

        Apply(currentAmount);
    }

    private float UpdateAlternating()
    {
        cycleTimer += Time.deltaTime;

        switch (cyclePhase)
        {
            case CyclePhase.OriginalHold:
                if (cycleTimer >= originalHoldTime)
                    ChangePhase(CyclePhase.ToHologram);

                return minAmount;

            case CyclePhase.ToHologram:
                if (Mathf.Abs(currentAmount - maxAmount) < 0.01f)
                    ChangePhase(CyclePhase.HologramHold);

                return maxAmount;

            case CyclePhase.HologramHold:
                if (cycleTimer >= hologramHoldTime)
                    ChangePhase(CyclePhase.ToOriginal);

                return maxAmount;

            case CyclePhase.ToOriginal:
                if (Mathf.Abs(currentAmount - minAmount) < 0.01f)
                    ChangePhase(CyclePhase.OriginalHold);

                return minAmount;
        }

        return minAmount;
    }

    private float UpdateFlicker()
    {
        flickerTimer += Time.deltaTime;

        if (flickerTimer >= flickerInterval)
        {
            flickerTimer = 0f;

            // Alterna entre los dos estados.
            flickerTarget = Mathf.Approximately(flickerTarget, minAmount)
                ? maxAmount
                : minAmount;
        }

        return flickerTarget;
    }

    private float UpdateOrganic()
    {
        float noise = Mathf.PerlinNoise(
            noiseOffset,
            Time.time * changeSpeed
        );

        float stretched = Mathf.Clamp01(Mathf.InverseLerp(0.25f, 0.75f, noise));

        float normalized = Mathf.Lerp(
            0.5f,
            stretched,
            noiseContrast
        );

        return Mathf.Lerp(minAmount, maxAmount, normalized);
    }

    private void ChangePhase(CyclePhase newPhase)
    {
        cyclePhase = newPhase;
        cycleTimer = 0f;
    }

    private void ResetBehavior()
    {
        velocity = 0f;
        cycleTimer = 0f;
        flickerTimer = 0f;

        cyclePhase = CyclePhase.OriginalHold;
        flickerTarget = minAmount;
    }

    public void Play()
    {
        mode = startMode;
        ResetBehavior();
    }

    public void SetAmount(float amount)
    {
        mode = HologramMode.Static;
        manualTarget = Mathf.Clamp(amount, minAmount, maxAmount);
    }

    public void ResumeAutoVary()
    {
        mode = HologramMode.Organic;
        ResetBehavior();
    }

    public void SetMode(HologramMode newMode)
    {
        mode = newMode;
        ResetBehavior();
    }

    private void Apply(float amount)
    {
        if (Mathf.Approximately(amount, lastApplied))
            return;

        lastApplied = amount;

        foreach (Material material in materials)
        {
            if (material != null)
                material.SetFloat(propertyId, amount);
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