using UnityEngine;
using System.Collections;

public class FadeBehaviorScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        StartFade();
    }

    public void StartFade()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float time = 0f;
        panel.alpha = 0f;

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(0f, 1f, time / fadeInDuration);
            yield return null;
        }

        panel.alpha = 1f;

        yield return new WaitForSeconds(waitDuration);

        time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(1f, 0f, time / fadeOutDuration);
            yield return null;
        }

        panel.alpha = 0f;
        fadeCoroutine = null;
    }
}
