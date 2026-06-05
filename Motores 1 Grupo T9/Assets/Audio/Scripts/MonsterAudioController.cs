using UnityEngine;

public class MonsterAudioController : MonoBehaviour
{
    [Header("idle")]
    [SerializeField] private AudioSource idleSource;

    [Header("Rugidos")]
    [SerializeField] private AudioSource vocalSource;
    [SerializeField] private AudioClip[] roarClips;
    [SerializeField] private AudioClip attackClip;

    private void Start()
    {
        
        if (idleSource != null)
        {
            idleSource.loop = true;
            if (!idleSource.isPlaying)
            {
                idleSource.Play();
            }
        }
    }

 
    public void PlayRoar()
    {
        if (vocalSource != null && roarClips.Length >0)
        {
            AudioClip randomRoar = roarClips[Random.Range(0, roarClips.Length)];
            vocalSource.PlayOneShot(randomRoar);
        }
    }

   
    public void PlayAttackSound()
    {
        if (vocalSource != null && attackClip != null)
        {
            vocalSource.PlayOneShot(attackClip);
        }
    }

    public void StopIdle()
    {
        if (idleSource != null)
        {
            idleSource.Stop();
        }
    }
}