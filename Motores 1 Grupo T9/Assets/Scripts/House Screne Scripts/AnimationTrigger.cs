using UnityEngine;
using System.Collections;


public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] TransitionToCave Script;
    [SerializeField] GameObject oldPlayer;
    [SerializeField] GameObject newPlayer;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float blackScreenDuration = 3f;
    [SerializeField] private float fadeOutDuration = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(FadeRoutine());
        }

    }
    public void DisableHud()
    {
        /*playerMovement.enabled = false;
        HUD.SetActive(false);
        PreviousHUD.SetActive(false);
        Debug.Log("asdasdsad");*/
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

            newPlayer.SetActive(true);
            oldPlayer.SetActive(false);
            Script.StartAnimation();
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
