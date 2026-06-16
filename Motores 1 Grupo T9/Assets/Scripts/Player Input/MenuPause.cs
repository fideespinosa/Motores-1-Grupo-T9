using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelPause;

    [SerializeField] AudioMixerSnapshot pauseSnapshot;
    [SerializeField] AudioMixerSnapshot normalState;

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
        pauseSnapshot.TransitionTo(0.5f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Continue()
    {
        panelPause.SetActive(false);
        Time.timeScale = 1f; 
        gamePaused = false;
        normalState.TransitionTo(0.5f);

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