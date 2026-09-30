using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;

    [Header("Musica para pantallas/menus")]
    [SerializeField] private AudioClip musicTrack;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (audioSource != null)
        {
            if (musicMixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = musicMixerGroup;
            }

            if (musicTrack != null)
            {
                audioSource.clip = musicTrack;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void FadeOutAndDestroy(float duration)
    {
        StartCoroutine(FadeRoutine(duration));
    }

    private IEnumerator FadeRoutine(float duration)
    {
        if (audioSource == null) yield break;

        float startVol = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        Destroy(gameObject);
    }
}