using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    [Header("Audio Config")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepsClips;

    [Header("Fisicas")]
    [SerializeField] private CharacterController characterController;

    [Tooltip("A partir de esta velocidad se considera caminando")]
    [SerializeField] private float speedThreshold;

    [Header("Ritmica")]
    [Tooltip("Segundos entre pasos")]
    [SerializeField] private float stepRate;
    private float stepTimer;

    private Vector3 lastPosition;
    private Coroutine footstepCoroutine;
    private bool isWalking = false;

    private void Start()
    {
        lastPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (characterController == null || footstepSource == null || footstepsClips.Length == 0) return;

        Vector3 currentHorizontalPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 lastHorizontalPos = new Vector3(lastPosition.x, 0f, lastPosition.z);
        float manualSpeed = Vector3.Distance(currentHorizontalPos, lastHorizontalPos) / Time.deltaTime;
        lastPosition = transform.position;

        bool currentlyWalking = manualSpeed > speedThreshold;
        if (currentlyWalking && !isWalking)
        {
            // Arrancó a caminar
            isWalking = true;
            footstepCoroutine = StartCoroutine(FootstepRoutine());
        }
        else if (!currentlyWalking && isWalking)
        {
            // Frenó
            isWalking = false;
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }

    }

    private IEnumerator FootstepRoutine()
    {
        
        while (isWalking)
        {
            PlayFootSteps();

          
            yield return new WaitForSeconds(stepRate);
        }
    }

    private void PlayFootSteps()

    {
        int randomIndex = Random.Range(0, footstepsClips.Length);
        AudioClip clipToPlay = footstepsClips[randomIndex];

        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.volume = Random.Range(0.8f, 1.0f);

        footstepSource.PlayOneShot(clipToPlay);
    }
}
