using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform droneBody;
    public Transform gimbalY;
    public Transform pivotX;

    [Header("Configuración")]
    public float sensitivity = 200f;
    public float clampAngle = 70f;

    [Header("Límite de cámara")]
    public float cameraYawLimit = 45f;
    public float droneTurnSpeed = 120f;

    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (MenuPausa.gamePaused)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -clampAngle, clampAngle);

        float nextYaw = cameraYaw + mouseX;

        if (Mathf.Abs(nextYaw) <= cameraYawLimit)
        {
            cameraYaw = nextYaw;
        }
        else
        {
            float direction = Mathf.Sign(mouseX);

            droneBody.Rotate(
                Vector3.up,
                direction * droneTurnSpeed * Time.deltaTime,
                Space.World
            );
        }

        gimbalY.localRotation = Quaternion.Slerp(
            gimbalY.localRotation,
            Quaternion.Euler(0f, cameraYaw, 0f),
            10f * Time.deltaTime
        );

        pivotX.localRotation = Quaternion.Slerp(
            pivotX.localRotation,
            Quaternion.Euler(cameraPitch, 0f, 0f),
            10f * Time.deltaTime
        );
    }
}