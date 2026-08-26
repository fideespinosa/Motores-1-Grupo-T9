using System.Collections;
using UnityEngine;
using TMPro;

public class TextPanelManager : MonoBehaviour
{
    public static TextPanelManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI displayedText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float typewriterSpeed = 0.03f;
    [SerializeField] private float punctuationPauseDuration = 0.25f;

    private Coroutine sequenceCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }
    }

    public void ShowText(string text)
    {
        if (panel == null || displayedText == null || panelCanvasGroup == null || string.IsNullOrEmpty(text)) return;

        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
        }
        sequenceCoroutine = StartCoroutine(ShowTextSequence(text));
    }

    private IEnumerator ShowTextSequence(string text)
    {
        panel.SetActive(true);
        displayedText.text = "";

        yield return Fade(panelCanvasGroup, panelCanvasGroup.alpha, 1f, fadeInDuration);

        yield return Typewriter(text);

        yield return new WaitForSeconds(displayDuration);

        yield return Fade(panelCanvasGroup, panelCanvasGroup.alpha, 0f, fadeOutDuration);

        panel.SetActive(false);
        sequenceCoroutine = null;
    }

    private IEnumerator Typewriter(string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            displayedText.text = text.Substring(0, i);

            if (i > 0 && i < text.Length)
            {
                char lastChar = text[i - 1];
                if (lastChar == ',' || lastChar == '.' || lastChar == ';' || lastChar == ':')
                {
                    yield return new WaitForSeconds(punctuationPauseDuration);
                    continue;
                }
            }

            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    public void HideText()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
        }
    }
}