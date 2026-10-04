using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class RatController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private float stoppingMargin = 0.3f;

    [Header("Idle")]
    [SerializeField] private Vector2 idleTimeRange = new Vector2(2f, 5f);
    [SerializeField] private Vector2 runTimeRange = new Vector2(4f, 8f);

    [Header("Detección del Player")]
    [SerializeField] private float triggerDistance = 1.2f;
    [SerializeField] private float triggerCooldown = 3f;
    [SerializeField] private string playerTag = "Player";

    [Header("Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip squishClip;
    [SerializeField] private TextMeshPro uiText;
    [SerializeField] private string message = "pise a la rata";
    [SerializeField] private float textDuration = 2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runParam = "IsRunning";

    private NavMeshAgent agent;
    private bool isIdle;
    private float stateTimer;
    private float lastTriggerTime = -999f;
    private Coroutine textRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = runSpeed;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (uiText != null) uiText.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartRunning();
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;

        if (isIdle)
        {
            if (stateTimer <= 0f) StartRunning();
        }
        else
        {
            bool arrived = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + stoppingMargin;
            if (arrived || stateTimer <= 0f)
            {
                if (Random.value < 0.6f) StartIdle();
                else StartRunning();
            }
        }

        CheckPlayerProximity();
    }

    private void StartIdle()
    {
        isIdle = true;
        agent.isStopped = true;
        stateTimer = Random.Range(idleTimeRange.x, idleTimeRange.y);
        SetRunAnim(false);
    }

    private void StartRunning()
    {
        isIdle = false;
        stateTimer = Random.Range(runTimeRange.x, runTimeRange.y);

        if (TryGetRandomPoint(out Vector3 point))
        {
            agent.isStopped = false;
            agent.SetDestination(point);
            SetRunAnim(true);
        }
        else
        {
            StartIdle();
        }
    }

    private bool TryGetRandomPoint(out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = transform.position + Random.insideUnitSphere * wanderRadius;
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = transform.position;
        return false;
    }

    private void CheckPlayerProximity()
    {
        if (Time.time - lastTriggerTime < triggerCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, triggerDistance);
        foreach (Collider col in hits)
        {
            if (col.CompareTag(playerTag))
            {
                lastTriggerTime = Time.time;
                OnStepped();
                break;
            }
        }
    }

    private void OnStepped()
    {
        if (audioSource != null && squishClip != null)
            audioSource.PlayOneShot(squishClip);

        if (uiText != null)
        {
            if (textRoutine != null) StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowText());
        }
    }

    private IEnumerator ShowText()
    {
        uiText.text = message;
        uiText.gameObject.SetActive(true);
        yield return new WaitForSeconds(textDuration);
        uiText.gameObject.SetActive(false);
        textRoutine = null;
    }

    private void SetRunAnim(bool running)
    {
        if (animator != null) animator.SetBool(runParam, running);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}