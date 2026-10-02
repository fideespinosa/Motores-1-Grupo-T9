using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVImageSequence : MonoBehaviour
{
    [System.Serializable]
    public class ImageData
    {
        public Sprite sprite;
        public float duration = 3f;
    }

    [SerializeField] private Image image;
    [SerializeField] private Image blackFade;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private List<ImageData> images;

    public void StartSequence()
    {
        image.gameObject.SetActive(true);

        Color color = blackFade.color;
        color.a = 0f;
        blackFade.color = color;
        blackFade.gameObject.SetActive(true);

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        for (int i = 0; i < images.Count; i++)
        {
            ImageData data = images[i];

            if (i > 0)
            {
                yield return Fade(1f);
            }

            image.sprite = data.sprite;
            image.gameObject.SetActive(true);

            yield return Fade(0f);

            yield return new WaitForSeconds(data.duration);
        }

        yield return Fade(1f);

        image.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color color = blackFade.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            blackFade.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        blackFade.color = color;
    }
}