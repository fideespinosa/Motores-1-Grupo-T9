using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class SFXTransitionManager : MonoBehaviour
{
    public static SFXTransitionManager Instance { get; private set; }

    [Header("Audio files")]
    [SerializeField] private AudioSource transAudioSource;
    [SerializeField] private AudioClip sfxToHouse;
    [SerializeField] private AudioClip sfxToCave;
    [SerializeField] private AudioSource sfxTransitionSource;
    [SerializeField] private AudioClip transitionSFX;

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

      
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionSFX()
    {
        if(sfxTransitionSource  != null)
        {
            sfxTransitionSource.clip = transitionSFX;
            sfxTransitionSource.Play();
        }
    }

   public void ToHouseFromShip()
    {
        if (transAudioSource  != null && sfxToHouse != null)
        {
           
            transAudioSource.clip = sfxToHouse;
            transAudioSource.Play();
        }
    }

    public void ToCaveFromHouse()
    {
        if (transAudioSource != null && sfxToCave != null)
        {

            transAudioSource.clip = sfxToCave;
            transAudioSource.Play();
        }
    }
    public void FadeOutAudios(AudioSource[] audiosToFade, float duration)
    {
        StartCoroutine(FadeOutAudiosRoutine(audiosToFade, duration));
    }
    private IEnumerator FadeOutAudiosRoutine(AudioSource[] audiosToFade, float duration)
    {
        if (audiosToFade == null || audiosToFade.Length == 0)
        {
            Debug.LogWarning("Se intentó hacer un Fade Out, pero el array de audios está vacío.");
            yield break;
        }

        Debug.Log($"Fade Out de {audiosToFade.Length} audios. Duración: {duration} segundos.");

        float[] startVolumes = new float[audiosToFade.Length];
        for (int i = 0; i < audiosToFade.Length; i++)
        {
            if (audiosToFade[i] != null)
                startVolumes[i] = audiosToFade[i].volume;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            
            elapsed += Time.unscaledDeltaTime;
            float lerpProgress = elapsed / duration;

            for (int i = 0; i < audiosToFade.Length; i++)
            {
                if (audiosToFade[i] != null)
                {
                    audiosToFade[i].volume = Mathf.Lerp(startVolumes[i], 0f, lerpProgress);
                }
            }
            yield return null;
        }

        for (int i = 0; i < audiosToFade.Length; i++)
        {
            if (audiosToFade[i] != null)
                audiosToFade[i].volume = 0f;
        }

        Debug.Log("Fade Out Ok.");
    }
}
