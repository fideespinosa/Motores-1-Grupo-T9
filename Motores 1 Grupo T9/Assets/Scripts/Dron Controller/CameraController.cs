using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    public Transform gimbalY; 
    public Transform pivotX;  

    public float sensitivity = 200f;
    public float clampAngle = 70f; 

    private float rotY = 0f;
    private float rotX = 0f;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;

        
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        
        gimbalY.localRotation = Quaternion.Slerp(gimbalY.localRotation, Quaternion.Euler(0f, rotY, 0f), 0.1f);
        pivotX.localRotation = Quaternion.Slerp(pivotX.localRotation, Quaternion.Euler(rotX, 0f, 0f), 0.1f);
    }
}