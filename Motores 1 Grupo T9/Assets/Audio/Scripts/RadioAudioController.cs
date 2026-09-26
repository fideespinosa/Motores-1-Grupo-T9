using UnityEngine;
using System.Collections;

public class RadioAudioController : MonoBehaviour
{
    [Header("Fuente de interferencia")]
    [SerializeField] private AudioSource radioInterference;

    [Header("Fuente de spots")]
    [SerializeField] private AudioSource radioSpots;
    [SerializeField] private AudioClip[] spotClips;

    [Header("Mínimos y máximos entre clips")]
    [SerializeField] private float minTimeSpots;
    [SerializeField] private float maxTimeSpots;

    private int currentIndex = 0;
    private bool isRadioActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRadioActive)
        {
            isRadioActive = true;

            if (radioInterference != null && !radioInterference.isPlaying)
            {
                radioInterference.loop = true;
                radioInterference.Play();
            }

            if (spotClips.Length > 0 && radioSpots != null)
            {
                StartCoroutine(PlaybackRoutine());
            }
        }
    }

    private IEnumerator PlaybackRoutine()
    {
        while (true)
        {
            AudioClip actualSpot = spotClips[currentIndex];
            radioSpots.clip = actualSpot;
            radioSpots.Play();

            yield return new WaitForSeconds(actualSpot.length);

            currentIndex = (currentIndex + 1) % spotClips.Length;

            float waitTime = Random.Range(minTimeSpots, maxTimeSpots);
            yield return new WaitForSeconds(waitTime);
        }
    }
}