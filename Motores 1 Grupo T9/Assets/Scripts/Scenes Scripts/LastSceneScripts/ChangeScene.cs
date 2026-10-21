using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string scene;
     public void MainMenu()
    {
        SceneManager.LoadScene(scene);
    }
}
