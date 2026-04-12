using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightScript : MonoBehaviour
{
    
    [SerializeField] private Light flashlight;
    [SerializeField] private bool isOn = false;

    [Header("Flicker Settings")]
    public bool enableFlicker = true;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.05f;

    private float baseIntensity;
    private float flickerTimer;

    void Start()
    {

        baseIntensity = flashlight.intensity;
        flashlight.enabled = isOn;
    }

    void Update()
    {
        if (isOn && enableFlicker)
        {
            Flicker();
        }
    }

    public void ToggleFlashlight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isOn = !isOn;
        flashlight.enabled = isOn;

        Debug.Log("on");
    }

    void Flicker()
    {
        flickerTimer -= Time.deltaTime;

        if (flickerTimer <= 0f)
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity);
            flashlight.intensity = baseIntensity * randomIntensity;

            flickerTimer = flickerSpeed;
        }
    }
}