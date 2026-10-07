using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlashlightFlicker : MonoBehaviour
{
    [SerializeField] private float steadyDurationMin = 4f;
    [SerializeField] private float steadyDurationMax = 8f;

    [SerializeField] private float flickerDurationMin = 0.5f;
    [SerializeField] private float flickerDurationMax = 1.5f;

    [SerializeField] private float flickerIntervalMin = 0.05f;
    [SerializeField] private float flickerIntervalMax = 0.15f;

    private Light spotLight;
    private Coroutine flickerCoroutine;

    private void Awake()
    {
        spotLight = GetComponent<Light>();
    }


    public void StartFlicker()
    {
        if (flickerCoroutine != null) return;
        flickerCoroutine = StartCoroutine(FlickerLoop());
    }

    public void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        spotLight.enabled = true;
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            spotLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(steadyDurationMin, steadyDurationMax));

            float flickerDuration = Random.Range(flickerDurationMin, flickerDurationMax);
            float elapsed = 0f;

            while (elapsed < flickerDuration)
            {
                spotLight.enabled = !spotLight.enabled;
                float interval = Random.Range(flickerIntervalMin, flickerIntervalMax);
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }

            spotLight.enabled = true;
        }
    }
}