using UnityEngine;

public class flicker : MonoBehaviour
{
    public Light targetLight;

    [Header("Intensidad")]
    public float baseIntensity = 2f;
    public float flickerAmount = 0.3f;

    [Header("Velocidad")]
    public float flickerSpeed = 15f;

    private float randomOffset;

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(
            Time.time * flickerSpeed,
            randomOffset
        );

        targetLight.intensity = baseIntensity + ((noise - 0.5f) * 2f * flickerAmount);
    }
}