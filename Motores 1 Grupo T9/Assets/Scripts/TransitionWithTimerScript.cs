using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionWithTimerScript : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] private string sceneToLoad;

    [Header("Tiempos")]
    [SerializeField] private float delayBeforeFade = 3f;
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float fadeInDuration = 1.5f;

    [Header("Panel negro")]
    [SerializeField] private CanvasGroup fadePanel;

    private void Start()
    {
        Cursor.visible = false;
        StartCoroutine(SceneTransitionRoutine());
    }

    private IEnumerator SceneTransitionRoutine()
    {

        fadePanel.alpha = 1f;
        fadePanel.blocksRaycasts = true;

        yield return new WaitForSeconds(delayBeforeFade);


        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / fadeOutDuration
            );

            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        yield return new WaitForSeconds(waitTime);

        fadePanel.blocksRaycasts = true;

        elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;

            fadePanel.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / fadeInDuration
            );

            yield return null;
        }

        fadePanel.alpha = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}