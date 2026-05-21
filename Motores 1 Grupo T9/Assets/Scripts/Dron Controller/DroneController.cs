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

    private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 inputMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        rb.centerOfMass = customCenterOfMass;
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
}