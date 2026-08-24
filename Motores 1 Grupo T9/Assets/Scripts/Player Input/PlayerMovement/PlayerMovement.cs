using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform de la cámara, hija del jugador.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float limiteAnguloVertical = 85f;
    [SerializeField] private bool invertirEjeY = false;

    [Header("Head Bob (cabeceo)")]
    [Tooltip("Activa o desactiva el efecto de cabeceo al caminar.")]
    [SerializeField] private bool habilitarHeadBob = true;
    [Tooltip("Qué tan rápido oscila la cabeza (más alto = pasos más rápidos).")]
    [SerializeField] private float bobFrecuencia = 6f;
    [Tooltip("Qué tan fuerte es el desplazamiento vertical del cabeceo.")]
    [SerializeField] private float bobAmplitudVertical = 0.04f;
    [Tooltip("Qué tan fuerte es el desplazamiento horizontal del cabeceo (efecto lateral leve).")]
    [SerializeField] private float bobAmplitudHorizontal = 0.02f;
    [Tooltip("Velocidad con la que la cámara vuelve a su posición al frenar.")]
    [SerializeField] private float bobSuavizado = 8f;

    private CharacterController controller;
    private Vector3 velocidad;
    private float rotacionX = 0f;

    private Vector3 posicionInicialCamara;
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
            posicionInicialCamara = cameraTransform.localPosition;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ManejarMouseLook();
        ManejarMovimiento();
    }

    private void ManejarMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertirEjeY ? 1f : -1f);

        transform.Rotate(Vector3.up * mouseX);

        rotacionX += mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -limiteAnguloVertical, limiteAnguloVertical);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        }
    }

    private void ManejarMovimiento()
    {
        bool enElSuelo = controller.isGrounded;

        if (enElSuelo && velocidad.y < 0f)
        {
            velocidad.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccion = transform.right * horizontal + transform.forward * vertical;
        direccion = Vector3.ClampMagnitude(direccion, 1f);

        controller.Move(direccion * walkSpeed * Time.deltaTime);

        velocidad.y += gravity * Time.deltaTime;
        controller.Move(velocidad * Time.deltaTime);

        AplicarHeadBob(direccion.magnitude, enElSuelo);
    }

    private void AplicarHeadBob(float inputMagnitud, bool enElSuelo)
    {
        if (cameraTransform == null) return;

        if (!habilitarHeadBob)
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, posicionInicialCamara, Time.deltaTime * bobSuavizado);
            return;
        }

        bool estaCaminando = inputMagnitud > 0.1f && enElSuelo;

        if (estaCaminando)
        {
            bobTimer += Time.deltaTime * bobFrecuencia;

            float offsetY = Mathf.Sin(bobTimer) * bobAmplitudVertical;
            float offsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmplitudHorizontal;

            Vector3 posicionObjetivo = posicionInicialCamara + new Vector3(offsetX, offsetY, 0f);
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, posicionObjetivo, Time.deltaTime * bobSuavizado);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, posicionInicialCamara, Time.deltaTime * bobSuavizado);
        }
    }
}