using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    public Camera humanCamera;
    public MonoBehaviour droneMovement;
    public MonoBehaviour droneCameraControl;
    public Camera droneCamera;

    private bool controllingDrone = false;
    private bool canSwitch = false; 
    public Canvas dronHUD;
    public Canvas playerDialogueHUD;

    void Start()
    {
        SetControl(false);
    }

    void Update()
    {
        
        if (!canSwitch) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            controllingDrone = !controllingDrone;
            SetControl(controllingDrone);
        }

        if (!controllingDrone && Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (ControlPanelManager.Instance != null)
                ControlPanelManager.Instance.ShowCurrentDialogue();
        }
    }

    
    public void BlockSwitching() => canSwitch = false;
    public void AllowSwitching() => canSwitch = true;

    public void SetControl(bool isDrone)
    {
        controllingDrone = isDrone;

        
        humanCamera.enabled = !isDrone;
        var humanAudio = humanCamera.GetComponent<AudioListener>();
        if (humanAudio != null) humanAudio.enabled = !isDrone;

        
        if (droneMovement != null) droneMovement.enabled = isDrone;
        if (droneCameraControl != null) droneCameraControl.enabled = isDrone;
        if (droneCamera != null) droneCamera.enabled = isDrone;
        if (dronHUD != null) dronHUD.gameObject.SetActive(isDrone);

        var droneAudio = droneCamera != null ? droneCamera.GetComponent<AudioListener>() : null;
        if (droneAudio != null) droneAudio.enabled = isDrone;
      
        if (AudioAmbienceController.Instance != null) //Agus: Cambio de ambientes sonoros.
        {
            if (isDrone)
            {
                AudioAmbienceController.Instance.ToDron();
            }
            else
            {
                AudioAmbienceController.Instance.ToShip();
            }
        }

        if (SFXManager.Instance != null && SFXManager.Instance.Transition != null)
        {
            SFXManager.Instance.Transition.PlayTransition(isDrone);
        }

        if (canSwitch)
        {
            Cursor.lockState = isDrone ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isDrone;
        }
    }
}