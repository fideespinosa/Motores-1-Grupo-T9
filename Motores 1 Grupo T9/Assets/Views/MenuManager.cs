using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StartGame(string escena)
    {
        SceneManager.LoadScene(escena);
    }
    public void EndGame()
    {
        Debug.Log("Sale del juego");
        Application.Quit();
    }

    public void EnterLastLevel()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "Level0");
        SceneManager.LoadScene(lastScene);
    }

}
