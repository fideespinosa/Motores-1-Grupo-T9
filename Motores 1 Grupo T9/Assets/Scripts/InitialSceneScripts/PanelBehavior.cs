using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class PanelBehavior : MonoBehaviour
{
    [SerializeField] float delayBeforeFade = 5f;
    [SerializeField]float fadeDuration = 3f;
    [SerializeField] float audioDuration = 33f;


    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
        StartCoroutine(FadeOut());

        Invoke(nameof(ChangeScene), audioDuration);
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

    void ChangeScene()
    {
        Color color = panelImage.color;
        color.a = 1f;
        panelImage.color = color;

        gameObject.SetActive(true);

        SceneManager.LoadScene("SecondScene");
    }
}