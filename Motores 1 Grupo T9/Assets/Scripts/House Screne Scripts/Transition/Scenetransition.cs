using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private string sceneToLoad;

    private void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneToLoad);
    }
}