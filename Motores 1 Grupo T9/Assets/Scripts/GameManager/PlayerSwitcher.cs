using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    

    public Camera humanCamera;

    
    public MonoBehaviour droneMovement; 
    public MonoBehaviour droneCameraControl; 
    public Camera droneCamera;

    private bool controllingDrone = false;

    [Header("HUDs")]
    public Canvas dronHUD;

    public Canvas playerDialogueHUD;

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

        if (!controllingDrone && Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Presiona la q");
            if (ControlPanelManager.Instance != null)
                ControlPanelManager.Instance.ShowCurrentDialogue();
        }
    }

    public void SetControl(bool isDrone)
    {
        
        controllingDrone = isDrone;

        
        humanCamera.enabled = !isDrone;
        var humanAudio = humanCamera.GetComponent<AudioListener>();
        if (humanAudio != null) humanAudio.enabled = !isDrone;

      
        droneMovement.enabled = isDrone;
        droneCameraControl.enabled = isDrone;
        droneCamera.enabled = isDrone;
        dronHUD.gameObject.SetActive(isDrone);

        var droneAudio = droneCamera.GetComponent<AudioListener>();
        if (droneAudio != null) droneAudio.enabled = isDrone;

       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
        this.enabled = true;

    }
}