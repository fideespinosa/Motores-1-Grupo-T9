using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSequence : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TextMeshProUGUI firstText;
    [SerializeField] private Image continueImage;

    [SerializeField] private Button menuButton;
    [SerializeField] private CanvasGroup buttonCanvasGroup;

    [Header("Tiempos")]
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float firstTextVisibleTime = 3f;
    [SerializeField] private float continueDelay = 1f;
    [SerializeField] private float buttonDelay = 3f;

    private void Start()
    {

        SetTextAlpha(firstText, 0);
        SetImageAlpha(continueImage, 0);

        buttonCanvasGroup.alpha = 0;
        menuButton.interactable = false;

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        yield return new WaitForSeconds(initialDelay);

        yield return FadeText(firstText, 0, 1);
        yield return new WaitForSeconds(firstTextVisibleTime);
        yield return FadeText(firstText, 1, 0);

        yield return new WaitForSeconds(continueDelay);

        yield return FadeImage(continueImage, 0, 1);

        yield return new WaitForSeconds(buttonDelay);

        yield return FadeCanvasGroup(buttonCanvasGroup, 0, 1);

        menuButton.interactable = true;
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            SetTextAlpha(text,
                Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration));

            yield return null;
        }

        SetTextAlpha(text, endAlpha);
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            SetImageAlpha(image,
                Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration));

            yield return null;
        }

        SetImageAlpha(image, endAlpha);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);

            yield return null;
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        canvasGroup.alpha = endAlpha;
    }

    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}