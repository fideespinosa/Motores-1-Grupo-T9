using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame(string escena)
    {
        SceneManager.LoadScene(escena);
    }
    public void EndGame()
    {
        Debug.Log("Sale del juego");
        Application.Quit();
    }
}
