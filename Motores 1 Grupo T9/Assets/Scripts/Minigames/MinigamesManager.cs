using UnityEngine;

public class MinigamesManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Minigames")]
    [SerializeField] GameObject lettersGame;

    [Header("UI Game")]
    [SerializeField] GameObject lettersPanel;

    [Header("Instructions Scripts")]
    [SerializeField] GameObject instructionsPanel;

    protected PlayerSwitcher switcher;

    private void Start()
    {
        if (switcher == null) switcher = Object.FindFirstObjectByType<PlayerSwitcher>();
    }

    public void StartLettersGame()
    {
        FreezeGame();

        if (!instructionsPanel.GetComponent<InstructionsScript>().ShowInstructions())
        {
            lettersPanel.SetActive(true);
            lettersGame.SetActive(true);
        }
    }

    public void EndLettersGame()
    {
        lettersPanel.SetActive(false);
        lettersGame.SetActive(false);
        UnfreezeGame();
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

        if (switcher != null)
        {
            
            switcher.BlockSwitching();
            switcher.SetControl(false);
        }

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Encuentro iniciado: Dron apagado. Control y cámara fija transferidos al astronauta.");
    }
}