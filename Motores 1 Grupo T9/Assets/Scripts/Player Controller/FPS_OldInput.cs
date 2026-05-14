using UnityEngine;

public class FPS_OldInput : MonoBehaviour
{
    public Transform playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;

    private float rotationX = 0;
    private bool canLook = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canLook) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    public void DisableCameraMovement()
    {
        canLook = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableCameraMovement()
    {
        canLook = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}