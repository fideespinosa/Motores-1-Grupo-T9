using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelPause; 

    public static bool gamePaused = false;

    

 
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void Pause()
    {
        panelPause.SetActive(true);
        Time.timeScale = 0f; 
        gamePaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Continue()
    {
        panelPause.SetActive(false);
        Time.timeScale = 1f; 
        gamePaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f; 
        gamePaused = false;

        
        SceneManager.LoadScene("MainMenu");
    }
}