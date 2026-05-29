using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] Image fadeImage;

    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float waitBeforeFadeOut = 2f;

    void Awake()
    {
        StartCoroutine(FadeIn());
    }

    

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;

        color.a = 1f;
        fadeImage.color = color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeOut()
    {
        Color color = fadeImage.color;

        color.a = 0f;
        fadeImage.color = color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
}