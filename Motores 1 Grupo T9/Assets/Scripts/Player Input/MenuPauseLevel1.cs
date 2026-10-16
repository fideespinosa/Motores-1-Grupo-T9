using GLTFast.Schema;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MenuPauseLevel1 : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelPause;

    [Header("HudDron")]
    [SerializeField] private CamDroneUI UIScript;


    public static bool gamePaused = false;
    private bool openOption = false;
    private bool openPause = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P) || Keyboard.current.pKey.isPressed)
        {
            Pause();
            
        }
    }
    public void Pause()
    {
        UIScript.SetVisibility(false);

        if (Cursor.visible && Cursor.lockState == CursorLockMode.None)
        {
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        gamePaused = true;
        openPause = true;


        if (panelPause != null)
        { panelPause.SetActive(true); }

        Time.timeScale = 0f;
    }

    public void Continue()
    {
        UIScript.SetVisibility(true);

        gamePaused = false;
        if (panelPause != null)
        { panelPause.SetActive(false); }

        Time.timeScale = 1f;

        // Ocultar y volver a bloquear el cursor para jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        gamePaused = false;

        
        SceneManager.LoadScene("MainMenu OK");
    }
    public void OnSliderChanged(float newValue)
    {
        // Le enviamos el valor puro (0 a 1) al manager global
        if (GlobalBrightnessManager.Instance != null)
        {
            GlobalBrightnessManager.Instance.UpdateBrightness(newValue);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}