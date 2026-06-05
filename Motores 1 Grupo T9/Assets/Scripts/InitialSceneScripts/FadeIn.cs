using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 3f;

    private Image panelImage;

    private void Start()
    {
        panelImage = GetComponent<Image>();
        StartCoroutine(FadeInNow());
    }

    IEnumerator FadeInNow()
    {
        Color color = panelImage.color;
        color.a = 1f;
        panelImage.color = color;

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

        // Opcional: desactivar el panel cuando termina
        gameObject.SetActive(false);
    }
}