using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [Header("Canales de Audio")]
    public AudioSource musicChannel;
    public AudioSource uiChannel;
    public AudioSource hoverChannel;
    [Range(0f, 1f)]
    public float uiVolume;

    [Header("Clips")]
    public AudioClip selectUi;
    public AudioClip acceptUi;
    public AudioClip exitUi;
    public AudioClip hoverUi;
    public AudioClip gameMusic;

    private Coroutine fadeCoroutine;    
    void Start()
    {
        if (musicChannel != null && gameMusic != null)
        {
            musicChannel.PlayOneShot(gameMusic);
        }

        if (hoverChannel != null)
        {
            hoverChannel.volume = 0f;
            hoverChannel.loop = true;
            hoverChannel.clip = hoverUi;
            hoverChannel.ignoreListenerPause = true;
        }
    }

    private void LastingAudioClip(AudioClip Click)
    {
        if (Click == null) return;

        GameObject eternalAudio = new GameObject("eternal_ " + Click.name);
        DontDestroyOnLoad(eternalAudio);

        AudioSource eternalSource = eternalAudio.AddComponent<AudioSource>();
        eternalSource.clip = Click;
        eternalSource.ignoreListenerPause = true;

        eternalSource.Play();

        Destroy(eternalAudio, Click.length);
    }

    public void PlayClickSelect()
    {
        if (uiChannel != null && selectUi != null)
        {
            uiChannel.PlayOneShot(selectUi);

        }
    }

    public void PlayClickAccept()
    {
        LastingAudioClip(acceptUi);
    }

    public void PlayClickExit()
    {
        LastingAudioClip(exitUi);
    }

    public void StartHover()
    {
        if (hoverChannel == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (!hoverChannel.isPlaying) hoverChannel.Play();

        fadeCoroutine = StartCoroutine(MakeFade(uiVolume)); 
    }

    public void StopHover()
    {
        if (hoverChannel == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(MakeFade(0f)); 
    }

    private IEnumerator MakeFade(float destinyVolume)
    {
        float fadeTime = 0.3f; 
        float volumenInicial = hoverChannel.volume;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.unscaledDeltaTime;
            hoverChannel.volume = Mathf.Lerp(volumenInicial, destinyVolume, time / fadeTime);
            yield return null;
        }

        hoverChannel.volume = destinyVolume;
        if (destinyVolume <= 0f) hoverChannel.Stop();
    }
}

