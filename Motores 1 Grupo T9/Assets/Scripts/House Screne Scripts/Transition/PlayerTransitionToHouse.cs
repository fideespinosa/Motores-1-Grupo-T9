using UnityEngine;
using System.Collections;

public class PlayerTransitionToHouse : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float blackScreenDuration = 3f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Players")]
    [SerializeField] private GameObject oldPlayer;
    [SerializeField] private GameObject oldPlayerCamera;
    [SerializeField] private GameObject newPlayer;

    [Header("Drone")]
    [SerializeField] private GameObject drone;

    [Header("Deactivate")]
    [SerializeField] private GameObject Ship;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject PreviousHUD;

    [Header("Activate")]
    [SerializeField] private GameObject Canvas;

    public void StartAnimation()
    {
        HUD.SetActive(false);
        PreviousHUD.SetActive(false);
        oldPlayer.GetComponent<FPS_OldInput>().enabled = false;
        oldPlayerCamera.GetComponent<CrosshairRaycast>().enabled = false;
        newPlayer.GetComponent<PlayerMovement>().enabled = false;

        fadePanel.alpha = 1f;
        fadePanel.blocksRaycasts = true;
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = true;

            StartCoroutine(FadeRoutine());
        }
    }

    private IEnumerator FadeRoutine()
    {
        Debug.Log("eeeeeeeeee");
        float time = 0f;

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, time / fadeInDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;

        if (oldPlayer != null)
            oldPlayer.SetActive(false);

        if (Canvas != null)
            Canvas.SetActive(true);

        if (newPlayer != null)
            newPlayer.SetActive(true);

        if (drone != null)
            drone.SetActive(false);

        yield return new WaitForSeconds(blackScreenDuration);

        time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, time / fadeOutDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
    }
}