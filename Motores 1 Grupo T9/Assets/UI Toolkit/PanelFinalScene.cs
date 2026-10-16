using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PanelFinalScene : MonoBehaviour
{
    [Header("FadeIn")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Texto en Menu")]
    [SerializeField] private SubtitleFinalScene subtitleSequence;
    [SerializeField] private GameObject buttonMain;
    [SerializeField] string sceneName;
    [SerializeField] float fadeInDuration = 3;
    [SerializeField] float buttonShow = 60;
    //[SerializeField] float qrImg = 50;
    [SerializeField] float creImgShow = 8;
    [SerializeField] float creImgClose = 30;
    [SerializeField] private GameObject itchioImg;
    [SerializeField] private GameObject igImg;
    [SerializeField] private GameObject credImg;
    [SerializeField] private GameObject credImg2;
    [SerializeField] private GameObject credImg3;

    private void Start()
    {
        panel.SetActive(false);
        buttonMain.SetActive(false);
        itchioImg.SetActive(false);
        igImg.SetActive(false);
        credImg.SetActive(false);
        credImg2.SetActive(false);
        credImg3.SetActive(false);
        subtitleSequence.Play();
        StartCoroutine(ShowButton());
    }
    public void OnMain()
    {
        panel.SetActive(true);
        StartCoroutine(FadePanel(0f, 1f, fadeInDuration));
    }
    IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(creImgShow);
        credImg.SetActive(true);
        credImg2.SetActive(true);
        credImg3.SetActive(true);

        yield return new WaitForSeconds(creImgClose);
        credImg.SetActive(false);
        credImg2.SetActive(false);
        credImg3.SetActive(false);

        yield return new WaitForSeconds(buttonShow);
        buttonMain.SetActive(true);
        itchioImg.SetActive(true);
        igImg.SetActive(true);

        

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
        SceneManager.LoadScene(sceneName);
    }
}
