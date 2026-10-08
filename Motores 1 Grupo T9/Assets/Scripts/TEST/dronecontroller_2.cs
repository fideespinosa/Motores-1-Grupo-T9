using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class DroneController_2 : MonoBehaviour
{
    public RoverWheel[] leftWheels;
    public RoverWheel[] rightWheels;

    public float motorForce = 300f;
    public float turnForce = 150f;

    [Header("Handling")]
    [SerializeField] private float inputAcceleration = 5f;
    [SerializeField] private float inputDeceleration = 8f;
    [SerializeField] private float maxSpeed = 0f;
    [SerializeField] private float maxTurnRate = 1.8f;
    [SerializeField] private float turnAcceleration = 6f;
    [SerializeField] private float highSpeedTurnFactor = 0.5f;
    [SerializeField] private float turnSpeedReference = 8f;
    [SerializeField] private float lateralGrip = 6f;

    [Header("Stability")]
    [SerializeField] private float stabilityStrength = 10f;
    [SerializeField] private float stabilityDamper = 2f;
    [SerializeField] private float normalSmoothing = 8f;
    [SerializeField] private float maxTiltAngularSpeed = 2.5f;
    [SerializeField] private Vector3 customCenterOfMass = new Vector3(0, -0.6f, 0);

    [Header("UI Glitch Transition")]
    [SerializeField] private CRTGlitchTransitionController glitchController;

    [Header("Colliders")]
    [SerializeField] private DroneColliderToggle colliderToggle;

    private Rigidbody rb;
    [SerializeField] private LayerMask groundLayer;
    private Vector2 inputMove;
    private float smoothedForward;
    private float smoothedTurn;
    private Vector3 smoothedTargetUp;
    private Coroutine unfreezeRoutine;
    [SerializeField] private DroneHitSequence hitSequence;

    RigidbodyConstraints originalConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = customCenterOfMass;
        originalConstraints = rb.constraints;
        smoothedTargetUp = transform.up;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

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
        float dt = Time.fixedDeltaTime;

        float forwardRate = Mathf.Abs(inputMove.y) > 0.01f ? inputAcceleration : inputDeceleration;
        float turnRate = Mathf.Abs(inputMove.x) > 0.01f ? inputAcceleration : inputDeceleration;
        smoothedForward = Mathf.MoveTowards(smoothedForward, inputMove.y, forwardRate * dt);
        smoothedTurn = Mathf.MoveTowards(smoothedTurn, inputMove.x, turnRate * dt);

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float drive = smoothedForward;
        if (maxSpeed > 0f && Mathf.Abs(drive) > 0.01f && Mathf.Sign(drive) == Mathf.Sign(forwardSpeed))
        {
            float taper = Mathf.Clamp01((maxSpeed - Mathf.Abs(forwardSpeed)) / (maxSpeed * 0.2f));
            drive *= taper;
        }

        float power = drive * motorForce;

        foreach (var wheel in leftWheels)
        {
            wheel.ApplyDriveForce(power);
        }

        foreach (var wheel in rightWheels)
        {
            wheel.ApplyDriveForce(power);
        }

        if (AnyWheelGrounded())
        {
            ApplySteering(forwardSpeed);
            ApplyLateralGrip();
        }

        ApplyGiroscopicStabiliy();
    }

    private bool AnyWheelGrounded()
    {
        foreach (var wheel in leftWheels)
        {
            if (wheel != null && wheel.IsGrounded) return true;
        }

        foreach (var wheel in rightWheels)
        {
            if (wheel != null && wheel.IsGrounded) return true;
        }

        return false;
    }

    private void ApplySteering(float forwardSpeed)
    {
        float speedRatio = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / Mathf.Max(0.01f, turnSpeedReference));
        float speedFactor = Mathf.Lerp(1f, highSpeedTurnFactor, speedRatio);

        float targetYawRate = smoothedTurn * maxTurnRate * speedFactor;
        Vector3 up = transform.up;
        float currentYawRate = Vector3.Dot(rb.angularVelocity, up);
        float newYawRate = Mathf.MoveTowards(currentYawRate, targetYawRate, turnAcceleration * Time.fixedDeltaTime);

        rb.angularVelocity += up * (newYawRate - currentYawRate);
    }

    private void ApplyLateralGrip()
    {
        Vector3 lateralVelocity = Vector3.Project(rb.linearVelocity, transform.right);
        rb.AddForce(-lateralVelocity * lateralGrip, ForceMode.Acceleration);
    }

    public void FreezeDrone()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        smoothedForward = 0f;
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

        smoothedTargetUp = Vector3.Slerp(smoothedTargetUp, targetUp, Time.fixedDeltaTime * normalSmoothing).normalized;

        Vector3 currentUp = transform.up;
        Vector3 yawVelocity = Vector3.Project(rb.angularVelocity, currentUp);
        Vector3 tiltVelocity = rb.angularVelocity - yawVelocity;

        Vector3 tiltError = Vector3.Cross(currentUp, smoothedTargetUp);
        Vector3 torque = tiltError * (stabilityStrength * stabilityStrength) - tiltVelocity * (stabilityStrength * stabilityDamper);
        rb.AddTorque(torque, ForceMode.Acceleration);

        yawVelocity = Vector3.ClampMagnitude(yawVelocity, maxTurnRate * 1.5f);
        tiltVelocity = Vector3.ClampMagnitude(tiltVelocity, maxTiltAngularSpeed);
        rb.angularVelocity = yawVelocity + tiltVelocity;
    }

    public void UnfreezeOnTab()
    {
        if (unfreezeRoutine != null) StopCoroutine(unfreezeRoutine);
        unfreezeRoutine = StartCoroutine(WaitForTab());
    }

    private IEnumerator WaitForTab()
    {
        yield return null;

        while (Keyboard.current == null || !Keyboard.current.tabKey.wasPressedThisFrame)
            yield return null;

        UnfreezeDrone();
        unfreezeRoutine = null;
    }
}