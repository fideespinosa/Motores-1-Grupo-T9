using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TransitionTimer : MonoBehaviour
{
    [SerializeField] float timer = 5;
    [SerializeField] int levelToChange = 0;
    void Start()
    {
        StartCoroutine(CTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CTimer()
    {
        yield return new WaitForSeconds(timer);
        ChooseScene();
    }

    void ChooseScene()
    {
        switch (levelToChange)
        {
            case 0:
                SceneManager.LoadScene("level0");
                break;

            case 1:
                SceneManager.LoadScene("level1");
                break;

            case 2:
                SceneManager.LoadScene("level2");
                break;

            default:
                SceneManager.LoadScene("level0");
                break;
        }
    }
}
