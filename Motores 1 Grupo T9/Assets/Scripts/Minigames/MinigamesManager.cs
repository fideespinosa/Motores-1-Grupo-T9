using UnityEditor;
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
        Debug.Log("lo llamo desde el manager");

        FreezeGame();

        if (!instructionsPanel.GetComponent<InstructionsScript>().ShowInstructions())
        {
            lettersPanel.SetActive(true);
            lettersGame.SetActive(true);
        }
    }

    public void EndLettersGame()
    {
        Debug.Log("terminamos");
        lettersPanel.SetActive(false);
        lettersGame.SetActive(false);
        UnfreezeGame();
    }

    public void UnfreezeGame()
    {
        player.GetComponent<FPS_OldInput>().EnableCameraMovement();
        if (switcher != null)
        {
            switcher.enabled = true;
            switcher.SetControl(true);
        }


        transform.root.gameObject.SetActive(false);

        Debug.Log("Mundo descongelado y control devuelto al Dron desde el manager.");
    }

    public void FreezeGame()
    {
        player.GetComponent<FPS_OldInput>().DisableCameraMovement();
        if (switcher != null)
        {
            switcher.SetControl(false);
            switcher.enabled = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Mundo congelado y desde el manager.");
    }

}
