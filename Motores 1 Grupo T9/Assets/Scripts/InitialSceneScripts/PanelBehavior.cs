using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PanelBehavior : MonoBehaviour
{
    [SerializeField] float delayBeforeFade = 5f;
    [SerializeField] float fadeDuration = 3f;
    [SerializeField] float audioDuration = 33f;
    [SerializeField] float blackScreenDuration = 3f;

    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();

        StartCoroutine(FadeOut());
        StartCoroutine(EndSceneSequence());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        Color color = panelImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            panelImage.color = color;

            yield return null;
        }

        color.a = 0f;
        panelImage.color = color;
    }

    IEnumerator EndSceneSequence()
    {
        yield return new WaitForSeconds(audioDuration);

        Color color = panelImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            panelImage.color = color;

            yield return null;
        }

        color.a = 1f;
        panelImage.color = color;

        yield return new WaitForSeconds(blackScreenDuration);

        SceneManager.LoadScene("SecondScene");
    }
}