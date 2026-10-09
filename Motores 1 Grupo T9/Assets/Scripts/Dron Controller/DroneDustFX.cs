using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneDustFX : MonoBehaviour
{
    [System.Serializable]
    public class DustEntry
    {
        public string name = "Polvo";
        public ParticleSystem system;
        [Tooltip("Intensidad mínima para que se dispare")]
        public float minDecel = 5f;
        [Tooltip("Intensidad máxima (0 = sin límite)")]
        public float maxDecel = 0f;
        public float countMultiplier = 1f;
    }

    [Header("Sistemas")]
    [SerializeField] private DustEntry[] entries;

    [Header("Detección")]
    [SerializeField] private float minSpeedForDust = 2f;
    [SerializeField] private float hardBrakeDecel = 8f;      
    [SerializeField] private float inputBrakeIntensity = 20f; 
    [SerializeField] private float dustCooldown = 0.15f;

    [Header("Cantidad")]
    [SerializeField] private int minParticles = 6;
    [SerializeField] private int maxParticles = 35;

    private Rigidbody rb;
    private float prevSpeed;
    private float lastDustTime = -10f;
    private bool wasBraking;

    void Awake() => rb = GetComponent<Rigidbody>();
    void OnEnable() => prevSpeed = 0f;

    void FixedUpdate()
    {
        if (rb.constraints == RigidbodyConstraints.FreezeAll)
        {
            prevSpeed = 0f;
            wasBraking = false;
            return;
        }

        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        float speed = flatVel.magnitude;
        float decel = (prevSpeed - speed) / Time.fixedDeltaTime;

        float forwardSpeed = Vector3.Dot(flatVel, transform.forward);
        bool inputBraking = Keyboard.current != null &&
            ((Keyboard.current.sKey.isPressed && forwardSpeed > minSpeedForDust) ||
             (Keyboard.current.wKey.isPressed && forwardSpeed < -minSpeedForDust));

        bool brakeStarted = inputBraking && !wasBraking;
        wasBraking = inputBraking;

        // La intensidad depende de la velocidad que llevabas, no del sentido
        float intensity = decel;
        if (brakeStarted) intensity = Mathf.Max(intensity, prevSpeed * 3f);

        if (prevSpeed > minSpeedForDust
            && intensity > hardBrakeDecel
            && Time.time - lastDustTime > dustCooldown)
        {
            EmitAll(intensity);
            lastDustTime = Time.time;
        }

        prevSpeed = speed;
    }

    void EmitAll(float intensity)
    {
        float t = Mathf.InverseLerp(hardBrakeDecel, 80f, intensity);
        int baseCount = Mathf.RoundToInt(Mathf.Lerp(minParticles, maxParticles, t));

        foreach (var e in entries)
        {
            if (e.system == null) continue;
            if (intensity < e.minDecel) continue;
            if (e.maxDecel > 0f && intensity >= e.maxDecel) continue;

            e.system.Emit(Mathf.Max(1, Mathf.RoundToInt(baseCount * e.countMultiplier)));
        }
    }
}