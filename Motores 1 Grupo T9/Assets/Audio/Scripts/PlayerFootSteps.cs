using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    [Header("Audio Config")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] woodClips;
    [SerializeField] private AudioClip[] mudClips;

    [Header("Fisicas")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speedThreshold;

    [Header("Ritmica")]
    [SerializeField] private float stepRate;

    private Vector3 lastPosition;
    private float stepTimer = 0f;

    public bool isInsideHouse = false;

    private void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (characterController == null || footstepSource == null) return;

        Vector3 currentHorizontalPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 lastHorizontalPos = new Vector3(lastPosition.x, 0f, lastPosition.z);
        float manualSpeed = Vector3.Distance(currentHorizontalPos, lastHorizontalPos) / Time.deltaTime;
        lastPosition = transform.position;

        bool currentlyWalking = manualSpeed > speedThreshold;

        if (stepTimer > 0f)
        {
            stepTimer -= Time.deltaTime;
        }

        if (currentlyWalking && stepTimer <= 0f)
        {
            PlayFootSteps();
            stepTimer = stepRate;
        }
    }

    private void PlayFootSteps()
    {
        AudioClip[] currentClips = isInsideHouse ? woodClips : mudClips;

        if (currentClips != null && currentClips.Length > 0)
        {
            int randomIndex = Random.Range(0, currentClips.Length);
            AudioClip clipToPlay = currentClips[randomIndex];

            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            footstepSource.volume = Random.Range(0.8f, 1.0f);

            footstepSource.PlayOneShot(clipToPlay);
        }
    }
}