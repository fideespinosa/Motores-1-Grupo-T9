using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    public RoverWheel[] leftWheels;
    public RoverWheel[] rightWheels;

    public float motorForce = 300f;
    public float turnForce = 150f;

    [Header("Turning")]
    [SerializeField] private float turnFactor = 1f;
    [SerializeField] private float turnRamp = 4f;
    [SerializeField] private float turnStartValue = 0.4f;
    [SerializeField] private float maxTurnRate = 1.5f;

    [SerializeField] private float stabilityStrength = 10f;
    [SerializeField] private float stabilityDamper = 2f;
    [SerializeField] private Vector3 customCenterOfMass = new Vector3(0, -0.6f, 0);

    [Header("UI Glitch Transition")]
    [SerializeField] private CRTGlitchTransitionController glitchController;

    [Header("Colliders")]
    [SerializeField] private DroneColliderToggle colliderToggle;

    private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 inputMove;
    private float smoothedTurn;
    private Coroutine unfreezeRoutine;
    [SerializeField] private DroneHitSequence hitSequence;

    RigidbodyConstraints originalConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = customCenterOfMass;
        originalConstraints = rb.constraints;
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
        if (Mathf.Abs(inputMove.x) > 0.01f && Mathf.Abs(smoothedTurn) < turnStartValue)
        {
            smoothedTurn = Mathf.Sign(inputMove.x) * turnStartValue;
        }

        smoothedTurn = Mathf.MoveTowards(smoothedTurn, inputMove.x, turnRamp * Time.fixedDeltaTime);

        float turnPower = smoothedTurn * motorForce * turnFactor;
        float leftForce = inputMove.y * motorForce + turnPower;
        float rightForce = inputMove.y * motorForce - turnPower;

        foreach (var wheel in leftWheels)
        {
            wheel.ApplyDriveForce(leftForce);
        }

        foreach (var wheel in rightWheels)
        {
            wheel.ApplyDriveForce(rightForce);
        }

        ApplyGiroscopicStabiliy();
        LimitYawRate();
    }

    private void LimitYawRate()
    {
        float yawRate = Vector3.Dot(rb.angularVelocity, transform.up);

        if (Mathf.Abs(yawRate) > maxTurnRate)
        {
            rb.angularVelocity -= transform.up * (yawRate - Mathf.Sign(yawRate) * maxTurnRate);
        }
    }

    public void FreezeDrone()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        smoothedTurn = 0f;

        if (colliderToggle != null)
            colliderToggle.SetColliders(false);
    }

    public void UnfreezeDrone()
    {
        if (unfreezeRoutine != null) { StopCoroutine(unfreezeRoutine); unfreezeRoutine = null; }
        rb.constraints = originalConstraints;

        if (colliderToggle != null)
            colliderToggle.SetColliders(true);
        if (hitSequence != null) hitSequence.ResetCamera();
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
    public void UnfreezeOnTab()
    {
        if (unfreezeRoutine != null) StopCoroutine(unfreezeRoutine);
        unfreezeRoutine = StartCoroutine(WaitForTab());
    }
    private IEnumerator WaitForTab()
    {
        yield return null; // ignora un TAB del mismo frame en que termino el minijuego

        while (Keyboard.current == null || !Keyboard.current.tabKey.wasPressedThisFrame)
            yield return null;

        UnfreezeDrone();
        unfreezeRoutine = null;
    }
}