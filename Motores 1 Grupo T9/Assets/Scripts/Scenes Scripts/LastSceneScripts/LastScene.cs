using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingSequence : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] TextMeshProUGUI firstText;
    [SerializeField] Image continueImage;
    [SerializeField] Button menuButton;
    [SerializeField] CanvasGroup buttonCanvasGroup;

    [Header("Tiempos")]
    [SerializeField] float sequenceStartDelay = 67f;
    [SerializeField] float initialDelay = 2f;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float firstTextVisibleTime = 3f;
    [SerializeField] float continueDelay = 1f;
    [SerializeField] float buttonDelay = 3f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetGraphicAlpha(firstText, 0f);
        SetGraphicAlpha(continueImage, 0f);

        buttonCanvasGroup.alpha = 0f;
        menuButton.interactable = false;

        StartCoroutine(Sequence());


    }
    private IEnumerator Sequence()
    {
        yield return new WaitForSeconds(sequenceStartDelay);
        yield return new WaitForSeconds(initialDelay);

        yield return Fade(0f, 1f, alpha => SetGraphicAlpha(firstText, alpha));
        yield return new WaitForSeconds(firstTextVisibleTime);
        yield return Fade(1f, 0f, alpha => SetGraphicAlpha(firstText, alpha));

        yield return new WaitForSeconds(continueDelay);

        yield return Fade(0f, 1f, alpha => SetGraphicAlpha(continueImage, alpha));

        yield return new WaitForSeconds(buttonDelay);

        yield return Fade(0f, 1f, alpha => buttonCanvasGroup.alpha = alpha);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        menuButton.interactable = true;
    }
    private IEnumerator Fade(float startAlpha, float endAlpha, Action<float> setAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            setAlpha(Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration));

            yield return null;
        }

        setAlpha(endAlpha);
    }
    private void SetGraphicAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

}