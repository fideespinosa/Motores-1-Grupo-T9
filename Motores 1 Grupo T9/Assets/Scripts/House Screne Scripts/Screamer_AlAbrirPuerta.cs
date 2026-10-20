using System.Collections;
using UnityEngine;

public class Screamer_AlAbrirPuerta : MonoBehaviour
{
    [SerializeField] GameObject Light1;
    [SerializeField] GameObject Light2;
    public float delayBeforeChase = 0.17f;
    public Collider monsterCollider;
    public Collider playerCollider;

    [Header("Story Flags")]
    public string requiredEventFlag = "PuertaBano_Cerrada";
    public string flagScareFinished = "SustoBano_Completado";

    [Header("Monster")]
    public GameObject monsterModel;
    public Transform targetPoint;
    public float movementSpeed = 40f;

    [Header("Audio")]
    public AudioSource monsterAudioSource;
    public AudioClip screamClip;

    [Header("Lights")]
    public LucesParpadeo lightsController;

    private bool scareRunning;
    private bool playerTouched;
    private bool canTouchPlayer;

    public void EjecutarSusto()
    {
        if (scareRunning) return;
        if (StoryFlagManager.Instance == null) return;
        if (!StoryFlagManager.Instance.HasFlag(requiredEventFlag)) return;
        if (StoryFlagManager.Instance.HasFlag(flagScareFinished)) return;

        StoryFlagManager.Instance.SetFlag(flagScareFinished);

        scareRunning = true;
        playerTouched = false;
        canTouchPlayer = false;

        StartCoroutine(ScreamerSequence());
    }

    private IEnumerator ScreamerSequence()
    {
        if (monsterModel == null || targetPoint == null)
        {
            Debug.LogWarning("Screamer: asigná Monster Model y Target Point.", this);
            scareRunning = false;
            yield break;
        }

        if (monsterAudioSource != null && screamClip != null)
            monsterAudioSource.PlayOneShot(screamClip);

        yield return new WaitForSeconds(delayBeforeChase);

        if (lightsController != null)
            lightsController.EjecutarParpadeo();
        monsterModel.SetActive(true);

        

        canTouchPlayer = true;

        while (!playerTouched)
        {
            Vector3 direction = targetPoint.position - monsterModel.transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                monsterModel.transform.rotation = Quaternion.Slerp(
                    monsterModel.transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime
                );
            }

            monsterModel.transform.position = Vector3.MoveTowards(
                monsterModel.transform.position,
                targetPoint.position,
                movementSpeed * Time.deltaTime
            );

            if (monsterCollider != null && playerCollider != null)
            {
                if (monsterCollider.bounds.Intersects(playerCollider.bounds))
                {
                    playerTouched = true;
                    canTouchPlayer = false;
                }
            }

            yield return null;
        }

        monsterModel.SetActive(false);


        scareRunning = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canTouchPlayer || playerTouched) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        playerTouched = true;
        canTouchPlayer = false;
    }

    public void OffFlickingLights()
    {
        Light1.SetActive(false);
        Light2.SetActive(false);
    }
}