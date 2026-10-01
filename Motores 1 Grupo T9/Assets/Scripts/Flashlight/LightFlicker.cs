using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Intensidad")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 2f;

    [Header("Velocidad")]
    [SerializeField] private float flickerSpeed = 10f;

    [Header("Modo")]
    [Tooltip("Si está activo, parpadea de forma suave (Perlin noise). Si no, salta de golpe entre valores aleatorios.")]
    [SerializeField] private bool smooth = true;

    private Light _light;
    private float _timer;
    private float _seed;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _seed = Random.value * 100f; // para que varias luces no parpadeen sincronizadas
    }

    private void Update()
    {
        if (smooth)
        {
            float noise = Mathf.PerlinNoise(_seed, Time.time * flickerSpeed);
            _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
        else
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _light.intensity = Random.Range(minIntensity, maxIntensity);
                _timer = 1f / flickerSpeed;
            }
        }
    }
}