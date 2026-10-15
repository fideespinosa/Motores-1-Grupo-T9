using Unity.VisualScripting;
using UnityEngine;

public class SFXTransitionManager : MonoBehaviour
{
    [Header("Audio files")]
    [SerializeField] private AudioSource transAudioSource;
    [SerializeField] private AudioClip sfxToHouse;
    [SerializeField] private AudioClip sfxToCave;

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
}
