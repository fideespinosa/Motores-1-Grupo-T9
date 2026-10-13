using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucesParpadeo : MonoBehaviour
{
    [Header("Lights")]
    public List<Light> lights = new List<Light>();

    [Header("Flicker Settings")]
    public float duration = 3f;
    public float minInterval = 0.04f;
    public float maxInterval = 0.15f;
    public float intensityMultiplier = 1.5f;

    [Header("Final State")]
    public bool leaveLightsOn = true;

    private readonly List<float> originalIntensities = new List<float>();
    private readonly List<bool> originalStates = new List<bool>();
    private Coroutine flickerCoroutine;

    public void EjecutarParpadeo()
    {
        if (lights.Count == 0)
        {
            Debug.LogWarning("ScreamerLucesParpadeo: no hay luces asignadas.", this);
            return;
        }

        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

        originalIntensities.Clear();
        originalStates.Clear();

        foreach (Light lightSource in lights)
        {
            originalIntensities.Add(lightSource != null ? lightSource.intensity : 0f);
            originalStates.Add(lightSource != null && lightSource.enabled);
        }

        flickerCoroutine = StartCoroutine(Parpadeo());
    }

    private IEnumerator Parpadeo()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            for (int i = 0; i < lights.Count; i++)
            {
                Light lightSource = lights[i];
                if (lightSource == null) continue;

                lightSource.enabled = Random.value > 0.5f;

                if (lightSource.enabled)
                    lightSource.intensity = originalIntensities[i] * intensityMultiplier;
            }

            float waitTime = Random.Range(
                Mathf.Max(0.01f, minInterval),
                Mathf.Max(minInterval, maxInterval)
            );

            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;
        }

        for (int i = 0; i < lights.Count; i++)
        {
            if (lights[i] == null) continue;

            lights[i].intensity = originalIntensities[i];
            lights[i].enabled = leaveLightsOn ? true : false;
        }

        flickerCoroutine = null;
    }
}