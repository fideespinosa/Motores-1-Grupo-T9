using UnityEngine;
using UnityEngine.AI;

public class MonsterAudioController : MonoBehaviour
{
    [Header("idle")]
    [SerializeField] private AudioSource idleSource;
    [SerializeField] private AudioClip idleClip;


    [Header("Rugidos")]
    [SerializeField] private AudioSource vocalSource;
    [SerializeField] private AudioClip[] roarClips;
    [SerializeField] private AudioClip attackClip;

    [Header("Persue")]
    [SerializeField] private AudioSource persueSource;
    [SerializeField] private AudioClip persueClip;

    [Header("Audio Config")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] mudClips;

    [Header("NavMesh y Ritmo")]
    [SerializeField] private NavMeshAgent agent; 
    [SerializeField] private float walkStepRate = 0.6f;
    [SerializeField] private float chaseStepRate = 0.3f;
    [SerializeField] private float speedThreshold = 0.1f;

    private float stepTimer = 0f;
    private bool isChasing = false;

    void Update()
    {
        if (footstepSource == null || agent == null) return;

        
        float currentSpeed = agent.velocity.magnitude;
        bool currentlyMoving = currentSpeed > speedThreshold;

        if (stepTimer > 0f)
        {
            stepTimer -= Time.deltaTime;
        }

        if (currentlyMoving && stepTimer <= 0f)
        {
            PlayFootSteps();

            stepTimer = (currentSpeed > 4.0f) ? chaseStepRate : walkStepRate;
        }
    }
    public void SetChaseMode(bool chasing)
    {
        isChasing = chasing;
    }

    private void PlayFootSteps()
    {

        if (mudClips != null && mudClips.Length > 0)
        {
            int randomIndex = Random.Range(0, mudClips.Length);
            AudioClip clipToPlay = mudClips[randomIndex];

            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.volume = Random.Range(0.8f, 1.0f);

            footstepSource.PlayOneShot(clipToPlay);
        }
    }


    public void IdlePlay()
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
    

    public void PlayPersue()
    {
        if (persueSource  != null)
        {
            StopIdle();
            persueSource.clip = persueClip;
            persueSource.loop = true;
            persueSource.Play();
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