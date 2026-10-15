using GLTFast.Schema;
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

    //[SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private UIDocument camDronUI;

    private VisualElement camDronVE;
    private Button playBtn;
    private Button closeButton;
    private Button menuButton;
    private Slider brightnessSlider;


    public static bool gamePaused = false;
    private bool openOption = false;
    private bool openPause = false;

    private void OnEnable()
    {
        var rootR = camDronUI.rootVisualElement;
        camDronVE = rootR.Q<VisualElement>("DronCam");

        string dron = camDronVE.visible.ToString();
        Debug.Log($"dron visible ? " + dron);
        //closeButton = rootVisualElement.Q<Button>("QuitPauseBtn");
        //playBtn = rootR.Q<Button>("PlayButton");
        //menuButton = rootVisualElement.Q<Button>("MenuPauseBtn");
        //brightnessSlider = rootVisualElement.Q<Slider>("BrightnessPauseSlider");

        //if (quitButton == null) Debug.LogError("No encontré QuitPauseBtn");
        //if (playBtn == null) Debug.LogError("No encontré OptionPauseBtn");
        //if (menuButton == null) Debug.LogError("No encontré MenuPauseBtn");
        //if (brightnessSlider == null) Debug.LogWarning("No encontré brightnessSlider");



        //if (playBtn != null) playBtn.clicked += Continue;
        //if (optionScreenBtn != null) optionScreenBtn.clicked += OptionScreen;
        //if (menuButton != null) menuButton.clicked += QuitToMainMenu;

        //if (brightnessSlider != null)
        //{
        //brightnessSlider.lowValue = 0f;
        //brightnessSlider.highValue = 1f;

        //if (GlobalBrightnessManager.Instance != null)
        //brightnessSlider.value = GlobalBrightnessManager.Instance.GetNormalizedBrightness();

        //brightnessSlider.RegisterValueChangedCallback(OnSliderChanged);
        //}
    }
    /*private void OnDisable()
    {
        if (playBtn != null) playBtn.clicked -= QuitGame;
        //if (optionScreenBtn != null) optionScreenBtn.clicked -= OptionScreen;
        //if (menuButton != null) menuButton.clicked -= QuitToMainMenu;
        //if (brightnessSlider != null)
            //brightnessSlider.UnregisterValueChangedCallback(OnSliderChanged);
    }*/
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P) || Keyboard.current.pKey.isPressed)
        {
            Pause();
            
        }
    }
    /*private void OnPauseTriggered(InputAction.CallbackContext context)
    {
        if (gamePaused)
            Continue();
        else
            Pause();
    }*/

    /*void LateUpdate()
    {
        if (gamePaused && Cursor.lockState != CursorLockMode.None)
        {
            Debug.LogWarning("Re-bloqueado mientras pausa");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }*/

    public void Pause()
    {
        //StartCoroutine(LockCursorNextFrame());
        gamePaused = true;
        openPause = true;

        camDronVE.style.display = DisplayStyle.None;
        string dron = camDronVE.visible.ToString();
        Debug.Log($"dron visible en pausa? " + dron);

        if (panelPause != null)
        { panelPause.SetActive(true); }

        //if (playerMovement != null)
        //{ playerMovement.enabled = false; }

        /// Liberar el cursor para poder usar el menú con el ratón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
      //  pauseSnapshot.TransitionTo(0.5f);

        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

    }

    public void Continue()
    {

        gamePaused = false;

        if (panelPause != null)
        { panelPause.SetActive(false); }

        //if (playerMovement != null)
        //{ playerMovement.enabled = true;  }

        if(camDronVE != null) camDronVE.style.display = DisplayStyle.Flex;

        Time.timeScale = 1f;
        //StartCoroutine(LockCursorNextFrame());


        //   normalState.TransitionTo(0.5f);

        // Ocultar y volver a bloquear el cursor para jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private System.Collections.IEnumerator LockCursorNextFrame()
    {
        yield return null; // un frame después
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        gamePaused = false;

        
        SceneManager.LoadScene("MainMenu OK");
    }
    private void OptionScreen()
    {
        openOption = true;
        //hudPauseVE.style.display = DisplayStyle.None;
        //optionPauseVE.style.display = DisplayStyle.Flex;
    }
    private void OnSliderChanged(ChangeEvent<float> evt)
    {
        // Le enviamos el valor puro (0 a 1) al manager global
        if (GlobalBrightnessManager.Instance != null)
        {
            GlobalBrightnessManager.Instance.UpdateBrightness(evt.newValue);
        }
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}