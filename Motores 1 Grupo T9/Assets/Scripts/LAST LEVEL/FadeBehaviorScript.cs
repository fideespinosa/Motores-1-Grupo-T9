using UnityEngine;
using System.Collections;

public class FadeBehaviorScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;


    private void OnEnable()
    {
        StartCoroutine(FadeRoutine());
    }
    public void StartFade()
    {
        Debug.Log("START FADE");
        Debug.Log("Duration: " + fadeInDuration);
        Debug.Log("Object active: " + gameObject.activeInHierarchy);

        StartCoroutine(FadeRoutine());
    }

    public IEnumerator FadeRoutine()
    {
        Debug.Log("toutinasd");

        float time = 0f;

        panel.alpha = 0f;

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(0f, 1f, time / fadeInDuration);

            Debug.Log("Alpha: " + panel.alpha);

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
    }
}