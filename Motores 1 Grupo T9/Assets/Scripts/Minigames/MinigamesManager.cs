using UnityEngine;

public class MinigamesManager : MonoBehaviour
{

    [SerializeField] ScreensManagerScript screensManager;

    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Minigames")]
    [SerializeField] GameObject lettersGame;

    [Header("UI Game")]
    [SerializeField] GameObject lettersPanel;

    [Header("Instructions Scripts")]
    [SerializeField] GameObject instructionsPanel;

    [Header("Spaceship objects")]
    [SerializeField] GameObject alarmLight;
    [SerializeField] GameObject screenWarningImage;

    protected PlayerSwitcher switcher;
    public bool isAlarmActive;


    private void Start()
    {
        if (switcher == null) switcher = Object.FindFirstObjectByType<PlayerSwitcher>();
        screensManager = screensManager.GetComponent<ScreensManagerScript>();
    }

    public void DronFailure()
    {
        isAlarmActive = true;

        if (switcher != null)
        {
            switcher.BlockSwitching();
            switcher.SetControl(false);
        }
        alarmLight.SetActive(true);
        screenWarningImage.SetActive(true);

        if (SFXManager.Instance != null && SFXManager.Instance.Alarm != null)
        {
            SFXManager.Instance.Alarm.SetAlarmState(true);
        }
    }
    public void StartLettersGame()
    {
        Debug.Log("arranco miniijuego");
        FreezeGame();

        if (!instructionsPanel.GetComponent<InstructionsScript>().ShowInstructions())
        {
            lettersPanel.SetActive(true);
            lettersGame.SetActive(true);
        }
    }

    public void EndLettersGame()
    {
        Debug.Log("termino del minijuego");
        isAlarmActive = false;
        alarmLight.SetActive(false);
        screenWarningImage.SetActive(false);
        lettersPanel.SetActive(false);
        lettersGame.SetActive(false);
        UnfreezeGame();
        screensManager.ClosePanelScreen1();
        
        
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level1")
        {
                foreach (EnemyMovement enemy in Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None))
            {
                enemy.ResetEnemy();
            }
        }


        if (SFXManager.Instance != null)
        {
            if (SFXManager.Instance.Alarm != null) SFXManager.Instance.Alarm.SetAlarmState(false);
            if (SFXManager.Instance.Minigame != null) SFXManager.Instance.Minigame.PlayFeedback(true);
        }
    }

    public void UnfreezeGame()
    {
        
        player.GetComponent<FPS_OldInput>().EnableCameraMovement();

        if (switcher != null)
        {
            
            switcher.AllowSwitching();
            switcher.SetControl(false);
        }

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Minijuego terminado: Cámara del astronauta desbloqueada y sistema listo.");
    }

    public void FreezeGame()
    {
        
        player.GetComponent<FPS_OldInput>().DisableCameraMovement();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Encuentro iniciado: Dron apagado. Control y cámara fija transferidos al astronauta.");
    }
}