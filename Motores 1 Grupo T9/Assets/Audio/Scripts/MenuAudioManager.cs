using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MenuAudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource uiChannel;
    [Range(0f, 1f)]
    [SerializeField] private float uiVolume;

    [Header("Clips")]
    [SerializeField] private AudioClip selectUi;
    [SerializeField] private AudioClip acceptUi;
    [SerializeField] private AudioClip exitUi;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    private Coroutine fadeCoroutine;    
   

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
}

