using GLTFast.Schema;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MenuPauseLevel2 : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelPause;

    [SerializeField] private PlayerMovement playerMovement;

    private VisualElement camDronVE;
    private Button playBtn;
    private Button closeButton;
    private Button menuButton;
    private Slider brightnessSlider;


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
        playerMovement.enabled = false;
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
        playerMovement.enabled = false;

        gamePaused = false;

        if (panelPause != null)
        { panelPause.SetActive(false); }

        if(camDronVE != null) camDronVE.style.display = DisplayStyle.Flex;

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
    public void OnSliderChanged(ChangeEvent<float> evt)
    {
        // Le enviamos el valor puro (0 a 1) al manager global
        if (GlobalBrightnessManager.Instance != null)
        {
            GlobalBrightnessManager.Instance.UpdateBrightness(evt.newValue);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}