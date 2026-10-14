using GLTFast.Schema;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MenuPause : MonoBehaviour
{
    //[Header("Interfaz")]
    //public GameObject panelPause;

    //[SerializeField] AudioMixerSnapshot pauseSnapshot;
    //[SerializeField] AudioMixerSnapshot normalState;
    [SerializeField] private GameObject crosshairPanel;
    [SerializeField] private GameObject eyesCanvas;
    [SerializeField] private PlayerMovement housePlayerMovement;

    private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private VisualElement pauseHudVE;
    private VisualElement hudPauseVE;
    private VisualElement optionPauseVE;
    private Button quitButton;
    private Button menuButton;
    private Button optionScreenBtn;
    private Slider brightnessSlider;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction pauseAction;

    public static bool gamePaused = false;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("Menu");
            pauseAction = uiMap.FindAction("Pause");
        }
    }
    private void OnEnable()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        pauseHudVE = rootVisualElement.Q<VisualElement>("MenuPauseVE");
        hudPauseVE = rootVisualElement.Q<VisualElement>("HudPauseVE");
        optionPauseVE = rootVisualElement.Q<VisualElement>("OptionPauseVE");

        pauseHudVE.style.display = DisplayStyle.None;
        hudPauseVE.style.display = DisplayStyle.None;
        optionPauseVE.style.display = DisplayStyle.None;

        quitButton = rootVisualElement.Q<Button>("QuitPauseBtn");
        optionScreenBtn = rootVisualElement.Q<Button>("OptionPauseBtn");
        menuButton = rootVisualElement.Q<Button>("MenuPauseBtn");
        brightnessSlider = rootVisualElement.Q<Slider>("brightnessSlider");

        if (quitButton == null) Debug.LogError("No encontré QuitPauseBtn");
        if (optionScreenBtn == null) Debug.LogError("No encontré OptionPauseBtn");
        if (menuButton == null) Debug.LogError("No encontré MenuPauseBtn");
        if (brightnessSlider == null) Debug.LogWarning("No encontré brightnessSlider");

        if (quitButton != null) quitButton.clicked += QuitGame;
        if (optionScreenBtn != null) optionScreenBtn.clicked += OptionScreen;
        if (menuButton != null) menuButton.clicked += QuitToMainMenu;

        if (brightnessSlider != null)
        {
            brightnessSlider.lowValue = 0f;
            brightnessSlider.highValue = 1f;

            if (GlobalBrightnessManager.Instance != null)
                brightnessSlider.value = GlobalBrightnessManager.Instance.GetNormalizedBrightness();

            brightnessSlider.RegisterValueChangedCallback(OnSliderChanged);
        }
    }
    private void OnDisable()
    {
        if (quitButton != null) quitButton.clicked -= QuitGame;
        if (optionScreenBtn != null) optionScreenBtn.clicked -= OptionScreen;
        if (menuButton != null) menuButton.clicked -= QuitToMainMenu;
        if (brightnessSlider != null)
            brightnessSlider.UnregisterValueChangedCallback(OnSliderChanged);
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gamePaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }
    /*private void OnPauseTriggered(InputAction.CallbackContext context)
    {
        if (gamePaused)
            Continue();
        else
            Pause();
    }*/

    void LateUpdate()
    {
        if (gamePaused && Cursor.lockState != CursorLockMode.None)
        {
            Debug.LogWarning("Re-bloqueado mientras pausa");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }

    public void Pause()
    {

        gamePaused = true;

        if (crosshairPanel != null)
        { crosshairPanel.SetActive(false); }

        if (eyesCanvas != null)
        { eyesCanvas.SetActive(false); }

        if (housePlayerMovement != null)
        { housePlayerMovement.enabled = false; }
        //panelPause.SetActive(true);
        pauseHudVE.style.display = DisplayStyle.Flex;
        hudPauseVE.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = true;

        Time.timeScale = 0f;
      //  pauseSnapshot.TransitionTo(0.5f);

        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

    }

    public void Continue()
    {

        gamePaused = false;

        if (crosshairPanel != null)
        { crosshairPanel.SetActive(true); }

        if (eyesCanvas != null)
        { eyesCanvas.SetActive(true); }

        if (housePlayerMovement != null)
        { housePlayerMovement.enabled = true;  }
        //panelPause.SetActive(false);
        pauseHudVE.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = false;

        Time.timeScale = 1f;
        StartCoroutine(LockCursorNextFrame());


        //   normalState.TransitionTo(0.5f);

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
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
        hudPauseVE.style.display = DisplayStyle.None;
        optionPauseVE.style.display = DisplayStyle.Flex;
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