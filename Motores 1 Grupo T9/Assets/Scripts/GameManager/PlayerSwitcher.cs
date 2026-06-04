using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    public MonoBehaviour humanCameraScript;
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
        // 1. Bloqueamos el Tab al arrancar el mapa
        canSwitch = false;
        controllingDrone = false;

        // 2. Apagamos MANUALMENTE los componentes del dron para que no pisen al jugador
        if (droneCamera != null) droneCamera.enabled = false;
        if (droneMovement != null) droneMovement.enabled = false;
        if (droneCameraControl != null) droneCameraControl.enabled = false;
        if (dronHUD != null) dronHUD.gameObject.SetActive(false);

        // 3. Encendemos los ojos del astronauta de forma explícita
        if (humanCamera != null)
        {
            humanCamera.enabled = true;
            var humanAudio = humanCamera.GetComponent<AudioListener>();
            if (humanAudio != null) humanAudio.enabled = true;
        }

        // 4. Aseguramos el cursor inicial en primera persona
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        Debug.Log(droneCameraControl.GetType().Name);
        controllingDrone = isDrone;

        humanCamera.enabled = !isDrone;
        var humanAudio = humanCamera.GetComponent<AudioListener>();
        if (humanAudio != null) humanAudio.enabled = !isDrone;

        if (droneMovement != null) droneMovement.enabled = isDrone;
        if (humanCameraScript != null) humanCameraScript.enabled = !isDrone; // bloquea la camara del player cuando se juega con el dron
        if (droneCameraControl != null) droneCameraControl.enabled = isDrone;
        if (droneCamera != null) droneCamera.enabled = isDrone;
        if (dronHUD != null) dronHUD.gameObject.SetActive(isDrone);

        var droneAudio = droneCamera != null ? droneCamera.GetComponent<AudioListener>() : null;
        if (droneAudio != null) droneAudio.enabled = isDrone;

        // Si canSwitch es false al principio, igual bloqueamos el cursor para el astronauta
        if (canSwitch)
        {
            Cursor.lockState = isDrone ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isDrone;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}