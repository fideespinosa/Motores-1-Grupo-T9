using UnityEngine;

public class PulsingGlow : MonoBehaviour
{
    [Header("Colores")]
    [SerializeField] private Color colorA = new Color(0f, 0.8f, 1f);
    [SerializeField] private Color colorB = new Color(0.6f, 0f, 1f);

    [Header("Intensidad (HDR)")]
    [SerializeField] private float intensityMin = 0.5f;
    [SerializeField] private float intensityMax = 4f;

    [Header("Latido")]
    [SerializeField] private float speed = 1.5f;
    [Tooltip("Mayor valor = pulso más marcado y breve, tipo latido")]
    [SerializeField] private float sharpness = 2f;

    [Header("Opcional")]
    [SerializeField] private Light linkedLight; // para que ilumine el entorno
    [SerializeField] private float lightIntensityMax = 3f;

    private Renderer rend;
    private MaterialPropertyBlock block;

    // URP/Standard usan _EmissionColor
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        // Necesario para que la emisión funcione
        rend.sharedMaterial.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        // 0 a 1 a 0, suave
        float t = (Mathf.Sin(Time.time * speed * Mathf.PI * 2f) + 1f) * 0.5f;
        t = Mathf.Pow(t, sharpness);

        Color color = Color.Lerp(colorA, colorB, t);
        float intensity = Mathf.Lerp(intensityMin, intensityMax, t);

        rend.GetPropertyBlock(block);
        block.SetColor(EmissionColorID, color * intensity);
        rend.SetPropertyBlock(block);

        if (linkedLight != null)
        {
            linkedLight.color = color;
            linkedLight.intensity = Mathf.Lerp(0f, lightIntensityMax, t);
        }
    }
}