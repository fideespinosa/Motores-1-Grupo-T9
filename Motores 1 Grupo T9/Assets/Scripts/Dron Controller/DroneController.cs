using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    
    public RoverWheel[] leftWheels;
    public RoverWheel[] rightWheels;

   
    public float motorForce = 300f;
    public float turnForce = 150f;

    
    [SerializeField] private float stabilityStrength = 10f; 
    [SerializeField] private float stabilityDamper = 2f;    
    [SerializeField] private Vector3 customCenterOfMass = new Vector3(0, -0.6f, 0);

    [Header("UI Glitch Transition")]
    [SerializeField] private CRTGlitchTransitionController glitchController;

    private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 inputMove;

    RigidbodyConstraints originalConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = customCenterOfMass;
        originalConstraints = rb.constraints;

        // Si no está asignado en el Inspector, busca el componente en la escena
        if (glitchController == null)
        {
            glitchController = FindFirstObjectByType<CRTGlitchTransitionController>();
        }
    }

    void Update()
    {
        float forward = 0;
        if (Keyboard.current.wKey.isPressed) forward = 1;
        if (Keyboard.current.sKey.isPressed) forward = -1;

        float turn = 0;
        if (Keyboard.current.dKey.isPressed) turn = 1;
        if (Keyboard.current.aKey.isPressed) turn = -1;

        inputMove = new Vector2(turn, forward);

        // Disparar la transición de Glitch al presionar la tecla G
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            TriggerGlitchTransition();
        }
    }

    void FixedUpdate()
    {
       
        float leftPower = inputMove.y + inputMove.x;
        float rightPower = inputMove.y - inputMove.x;

        foreach (var wheel in leftWheels)
        {
            wheel.ApplyDriveForce(leftPower * motorForce);
        }

        foreach (var wheel in rightWheels)
        {
            wheel.ApplyDriveForce(rightPower * motorForce);
        }

        
        ApplyGiroscopicStabiliy();
    }

    public void FreezeDrone()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnfreezeDrone()
    {
        rb.constraints = originalConstraints;
    }
    private void ApplyGiroscopicStabiliy()
    {
        
        Vector3 targetUp = Vector3.up;

        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2f, groundLayer))
        {
            
            targetUp = hit.normal;
        }

        
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityDamper / stabilityStrength, rb.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, targetUp);

        
        rb.AddTorque(torqueVector * (stabilityStrength * stabilityStrength), ForceMode.Acceleration);

        
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, 4f);
    }
    /// <summary>
    /// Dispara la transición de glitch. En el punto máximo de la interferencia congela el dron brevemente.
    /// </summary>
    public void TriggerGlitchTransition()
    {
        if (glitchController != null)
        {
            // Congelar controles/físicas antes de romper la pantalla
            FreezeDrone();

            glitchController.TriggerGlitchTransition(() =>
            {
                Debug.Log("Pico del glitch: Cambiando estado o vista del dron.");
                // Restablecer movimiento en el momento cumbre de la transición
                UnfreezeDrone();
            });
        }
        else
        {
            Debug.LogWarning("CRTGlitchTransitionController no está asignado en DroneController.");
        }
    }
}