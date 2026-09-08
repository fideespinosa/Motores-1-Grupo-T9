using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleSequencePlayer : MonoBehaviour
{
    [System.Serializable]
    public class SubtitleLine
    {
        [TextArea]
        public string text;
        public float displayDuration = 3f;
    }

    [SerializeField] private TextMeshProUGUI displayedText;

    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [SerializeField] private List<SubtitleLine> subtitles;

    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;


    //cuando se quiera llamar a esa secunecia de texto, hay que referenciar el objeto radio
    // y llamar al metodp play();
    public void Play()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            yield return FadePanel(0f, 1f, fadeInDuration);
        }


        foreach (SubtitleLine line in subtitles)
        {
            displayedText.text = line.text;

            yield return FadeText(0f, 1f, fadeInDuration);
            yield return new WaitForSeconds(line.displayDuration);

            if (line != subtitles[subtitles.Count - 1])
            {
                yield return FadeText(1f, 0f, fadeOutDuration);
            }
        }
        yield return FadePanel(1f, 0f, fadeOutDuration);
        panel.SetActive(false);

        Debug.Log("termino textooo");
    }

    private IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            displayedText.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        displayedText.alpha = to;
    }

    private IEnumerator FadePanel(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        panelCanvasGroup.alpha = to;
    }
}
