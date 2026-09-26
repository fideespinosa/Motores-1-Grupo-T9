using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform de la cámara, hija del jugador.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;
    [Tooltip("Si está activo, se puede correr manteniendo Shift.")]
    [SerializeField] private bool enableRunning = false;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalAngleLimit = 85f;
    [SerializeField] private bool invertYAxis = false;

    [Header("Head Bob")]
    [Tooltip("Activa o desactiva el efecto de cabeceo al caminar.")]
    [SerializeField] private bool enableHeadBob = true;
    [Tooltip("Qué tan rápido oscila la cabeza (más alto = pasos más rápidos).")]
    [SerializeField] private float bobFrequency = 6f;
    [Tooltip("Qué tan fuerte es el desplazamiento vertical del cabeceo.")]
    [SerializeField] private float bobVerticalAmplitude = 0.04f;
    [Tooltip("Qué tan fuerte es el desplazamiento horizontal del cabeceo (efecto lateral leve).")]
    [SerializeField] private float bobHorizontalAmplitude = 0.02f;
    [Tooltip("Velocidad con la que la cámara vuelve a su posición al frenar.")]
    [SerializeField] private float bobSmoothing = 8f;
    [Tooltip("Multiplica la frecuencia y amplitud del cabeceo mientras se está corriendo.")]
    [SerializeField] private float runBobMultiplier = 1.6f;

    private CharacterController controller;
    private Vector3 velocity;
    private float rotationX = 0f;

    private Vector3 initialCameraPosition;
    private float bobTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            initialCameraPosition = cameraTransform.localPosition;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertYAxis ? 1f : -1f);

        transform.Rotate(Vector3.up * mouseX);

        rotationX += mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalAngleLimit, verticalAngleLimit);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = Vector3.ClampMagnitude(direction, 1f);

        bool isRunning = enableRunning && Input.GetKey(runKey) && direction.magnitude > 0.1f;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        controller.Move(direction * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        ApplyHeadBob(direction.magnitude, isGrounded, isRunning);
    }

    private void ApplyHeadBob(float inputMagnitude, bool isGrounded, bool isRunning)
    {
        if (cameraTransform == null) return;

        if (!enableHeadBob)
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, initialCameraPosition, Time.deltaTime * bobSmoothing);
            return;
        }

        bool isWalking = inputMagnitude > 0.1f && isGrounded;

        if (isWalking)
        {
            float bobMultiplier = isRunning ? runBobMultiplier : 1f;

            bobTimer += Time.deltaTime * bobFrequency * bobMultiplier;

            float offsetY = Mathf.Sin(bobTimer) * bobVerticalAmplitude * bobMultiplier;
            float offsetX = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude * bobMultiplier;

            Vector3 targetPosition = initialCameraPosition + new Vector3(offsetX, offsetY, 0f);
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, targetPosition, Time.deltaTime * bobSmoothing);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, initialCameraPosition, Time.deltaTime * bobSmoothing);
        }
    }
}