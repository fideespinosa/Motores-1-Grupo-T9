using UnityEngine;

public class DroneEngineAudio : MonoBehaviour
{
   
    [SerializeField] private Rigidbody droneRigidbody;

    [Header("(Motor / Zumbido")]
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Fricción / Deslizamiento")]
    [SerializeField] private AudioSource frictionSource;
    [SerializeField] private float maxFrictionVolume = 1.0f;

    [Header("Giro")]
    [SerializeField] private AudioSource rotationSource;
    [SerializeField] private float maxRotationVolume = 1.0f;
    [SerializeField] private float maxAngularSpeed = 3f;

    [Header("Choques")]
    [SerializeField] private AudioSource impactSource;
    [SerializeField] private AudioClip impactClips;
    [SerializeField] private float collisionThreshold = 1.5f;

    private void Update()
    {
        if (droneRigidbody == null) return;

        
        float currentSpeed = droneRigidbody.linearVelocity.magnitude; 
        float speedNormalized = Mathf.Clamp01(currentSpeed / maxSpeed);

        if (engineSource != null)
            engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);

        if (frictionSource != null)
            frictionSource.volume = Mathf.Lerp(0f, maxFrictionVolume, speedNormalized);

      
        if (rotationSource != null)
        {
            
            float currentAngularSpeed = droneRigidbody.angularVelocity.magnitude;
            float angularNormalized = Mathf.Clamp01(currentAngularSpeed / maxAngularSpeed);

            
            rotationSource.volume = Mathf.Lerp(0f, maxRotationVolume, angularNormalized);
        }
    }


    
    private void OnCollisionEnter(Collision collision)
    {
    
        if (!this.enabled) return;

        if (Time.timeSinceLevelLoad < 1f) return;

        if (impactSource == null || impactClips == null) return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > collisionThreshold)
        {
            float impactVolume = Mathf.Clamp01(impactForce / 10f);
            impactSource.PlayOneShot(impactClips, impactVolume);
        }
    }

    private void OnDisable()
    {
        
        if (engineSource != null) engineSource.Stop();
        if (frictionSource != null) frictionSource.Stop();
        if (rotationSource != null) rotationSource.Stop();
    }

    private void OnEnable()
    {
        
        if (frictionSource != null) frictionSource.volume = 0f;
        if (rotationSource != null) rotationSource.volume = 0f;

      
        if (engineSource != null && engineSource.clip != null) engineSource.Play();
        if (frictionSource != null && frictionSource.clip != null) frictionSource.Play();
        if (rotationSource != null && rotationSource.clip != null) rotationSource.Play();
    }
}