using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionWithTimerScript : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] private string sceneToLoad;

    [Header("Tiempos")]
    [SerializeField] private float initialFadeDuration = 1.5f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float finalFadeDuration = 1.5f;

    [Header("Panel negro")]
    [SerializeField] private CanvasGroup fadePanel;

    private void Start()
    {
        StartCoroutine(SceneTransitionRoutine());
    }

    private IEnumerator SceneTransitionRoutine()
    {
        fadePanel.alpha = 1f;
        fadePanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < initialFadeDuration)
        {
            elapsed += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / initialFadeDuration
            );

            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        yield return new WaitForSeconds(waitTime);

        fadePanel.blocksRaycasts = true;

        elapsed = 0f;

        while (elapsed < finalFadeDuration)
        {
            elapsed += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / finalFadeDuration
            );

            yield return null;
        }

        fadePanel.alpha = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}