using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BlackPanelToGame : MonoBehaviour
{
    [SerializeField] private float delayTime = 5f;
    [SerializeField] private string sceneName;

    private void OnEnable()
    {
        StartCoroutine(ChangeSceneAfterTime());
    }

    IEnumerator ChangeSceneAfterTime()
    {
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene(sceneName);
    }
}