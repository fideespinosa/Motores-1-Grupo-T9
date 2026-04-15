using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    
    public MonoBehaviour humanMovement;
    public Camera humanCamera;

    
    public MonoBehaviour droneMovement; 
    public MonoBehaviour droneCameraControl; 
    public Camera droneCamera;

    private bool controllingDrone = false;

    void Start()
    {
        SetControl(false);
    }

    void Update()
    {
        
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            controllingDrone = !controllingDrone;
            SetControl(controllingDrone);
        }
    }

    void SetControl(bool isDrone)
    {
      
        humanMovement.enabled = !isDrone;
        humanCamera.enabled = !isDrone;
        humanCamera.GetComponent<AudioListener>().enabled = !isDrone;

        
        droneMovement.enabled = isDrone;
        droneCameraControl.enabled = isDrone;
        droneCamera.enabled = isDrone;
        droneCamera.GetComponent<AudioListener>().enabled = isDrone;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}