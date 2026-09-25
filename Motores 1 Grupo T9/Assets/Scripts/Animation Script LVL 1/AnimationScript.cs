using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationScript : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject PreviousHUD;

    [Header("Fog")]
    [SerializeField] private GameObject fog;

    [Header("Blur")]
    [SerializeField] private GameObject blur;
    [SerializeField] private float blurFadeInDuration = 1f;
    [SerializeField] private float blurFadeOutDuration = 1f;

    [Header("Puerta")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator childHouseDoor;
    [SerializeField] private GameObject shipDoor;
    [SerializeField] private GameObject houseDoor;

    [Header("Ojos")]
    [SerializeField] private Animator Eyes;

    public void ChangeDoor()
    {
        houseDoor.SetActive(true);
        shipDoor.SetActive(false);
    }
    public void DisableMovement()
    {
        playerMovement.enabled = false;
        HUD.SetActive(false);
        PreviousHUD.SetActive(false);
        Debug.Log("asdasdsad");
    }

    public void EnableMovement()
    {
        playerMovement.enabled = true;
        HUD.SetActive(true);
    }

    public void FogStart()
    {
        fog.SetActive(true);
    }
    public void FogFade()
    {
        if (fog != null)
        {
            fogTest fogScript = fog.GetComponent<fogTest>();

            if (fogScript != null)
            {
                fogScript.FadeOut();
            }
        }
    }

    public void PlayDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Play");
        }
    }

    public void PlayEyes()
    {
        if (Eyes != null)
        {
            Eyes.SetTrigger("Play2");
        }
    }

    public void StartBlur()
    {
        if (blur != null)
        {

            blur.SetActive(true);

            Image image = blur.GetComponent<Image>();

            if (image != null)
            {
                StartCoroutine(FadeBlur(image, 1f, blurFadeInDuration));
            }
        }
    }

    public void OpenDoor()
    {
        if (childHouseDoor != null)
        {
            childHouseDoor.SetTrigger("Play");
        }
    }
    public void FadeOutBlur()
    {
        if (blur != null)
        {
            Image image = blur.GetComponent<Image>();

            if (image != null)
            {
                StartCoroutine(FadeBlur(image, 0f, blurFadeOutDuration));
            }
        }
    }

    private IEnumerator FadeBlur(Image image, float targetAlpha, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float progress = Mathf.Clamp01(time / duration);

            color.a = Mathf.Lerp(startAlpha, targetAlpha, progress);
            image.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }
}