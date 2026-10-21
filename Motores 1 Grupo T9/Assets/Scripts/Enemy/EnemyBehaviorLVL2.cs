using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class EnemyBehaviorLVL2 : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("Referencias base")]
    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] GameObject staticImage;
    [SerializeField] MonsterAudioController audioController;
    [SerializeField] GameObject panelFadeScript;

    [Header("Animaciones")]
    [SerializeField] private float delayAfterScream = 0.5f;

    [Header("Intro")]
    [SerializeField] private Transform introTarget;            // hacia dónde camina durante el Scream
    [SerializeField] private float introMoveSpeed = 2f;        // velocidad del avance
    [SerializeField] private float introArriveDistance = 0.3f; // distancia a la que se considera "llegó"
    private float runSpeed;
    private bool introMoving = false;

    [Header("Escala")]
    [SerializeField] private Vector3 screamScale = new Vector3(1.5f, 1.5f, 1.5f); // X, Y, Z durante el Scream
    [SerializeField] private float scaleTransitionTime = 0.3f; // 0 = cambio instantáneo
    private Vector3 originalScale;
    private Coroutine scaleRoutine;

    [Header("Viewpoint")]
    [SerializeField] private Transform viewPoint;

    [Header("Jumpscare / Muerte")]
    [SerializeField] private GameObject playerObject;
    private MonoBehaviour playerMovementScript;
    private Animator playerDeathAnimator;

    [Header("Cámara")]
    [SerializeField] private CameraSequenceController cameraController;
    [SerializeField] private float cameraLookSpeed = 5f;
    [SerializeField] FlashlightFlicker flashlightFlicker;

    [Header("Muerte - End")]
    [SerializeField] private string endStateName = "End";
    [SerializeField] private float endCrossFade = 0.1f;
    [SerializeField] private bool waitForEndEvent = false;   // true = espera Animation Event
    [SerializeField] private float endAnimationDelay = 2f;   // si no usa evento

    [Header("Muerte - Shake")]
    [SerializeField] private Transform shakeTarget;          // la camara del jugador
    [SerializeField] private float shakeMagnitude = 0.08f;
    [SerializeField] private float shakeDuration = 1.5f;

    [Header("Muerte - Cámara 'te come'")]
    [SerializeField] private float pullBackDistance = 0.8f;  // cuanto se aleja
    [SerializeField] private float pullBackTime = 0.4f;
    [SerializeField] private float lungeStartDelay = 1.5f;   // segundos desde que te atrapa hasta que se lanza (si no usa evento)
    [SerializeField] private float lungeTime = 0.35f;        // que tan rápido te "come"
    [SerializeField] private float lungeStopDistance = 0.3f; // a qué distancia del viewPoint frena

    private bool lungeEvent;

    [Header("Muerte - Fade / Música / Escena")]
    [SerializeField] private CanvasGroup deathFadeGroup;     // Image negra con CanvasGroup (alpha 0)
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AudioSource blackScreenSource;
    [SerializeField] private AudioClip blackScreenMusic;
    [SerializeField] private float blackHoldTime = 3f;
    [SerializeField] private string deathSceneName = "Gracias por jugar escena";

    private bool endAnimFinished;

    bool run = false;
    private bool jumpscareActive = false;
    private bool introSequenceActive = false;
    private bool contactEnabled = false; // bloquea el trigger hasta que termine el intro

    public bool IntroDone { get; private set; }
    public bool ScreamEnded { get; private set; }

    void Start()
    {
        originalScale = transform.localScale;
        agent = GetComponent<NavMeshAgent>();
        audioController = GetComponent<MonsterAudioController>();
        runSpeed = agent.speed;   // velocidad de correr = la del NavMeshAgent en el inspector
        agent.enabled = false;    // el agente se activa recién al empezar a correr

        if (playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerDeathAnimator = playerObject.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Intro: se mueve por script (sin NavMesh) hacia introTarget
        if (introMoving && introTarget != null)
        {
            Vector3 dir = introTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);

            transform.position = Vector3.MoveTowards(
                transform.position,
                introTarget.position,
                introMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, introTarget.position) <= introArriveDistance)
            {
                introMoving = false;
                Debug.Log($"[{Time.time:F2}] llegó al introTarget");
            }
        }

        // Persecución: con NavMeshAgent
        if (run)
        {
            Vector3 flat = player.position - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(flat);

            agent.destination = player.position;
        }
    }

    // ---------- FASE 1: Spawn / intro ----------

    public void StartScreaming()
    {
        Debug.Log("StartScreaming llamado");
        introSequenceActive = true;
        animator.SetTrigger("StartIntro"); // Idle -> Scream

        if (introTarget != null)
            introMoving = true;

        if (GameMusicManager.Instance != null)
            GameMusicManager.Instance.SetCombatState(true);
    }

    // Animation Event en el frame del grito
    public void PlayScreamRoar()
    {
        if (audioController != null)
            audioController.PlayRoar();
    }

    // Animation Event al final del clip Scream
    public void OnScreamEnd()
    {
        Debug.Log($"[{Time.time:F2}] OnScreamEnd");
        if (jumpscareActive)
            OnDeathScreamEnd();
        else if (introSequenceActive)
        {
            ScreamEnded = true; // el jugador se libera acá
            StartCoroutine(IntroToRunRoutine());
        }
    }

    private IEnumerator IntroToRunRoutine()
    {
        introSequenceActive = false;

        // si el clip terminó antes de que llegue al punto, espera a que llegue
        yield return new WaitUntil(() => !introMoving);

        yield return new WaitForSeconds(delayAfterScream);

        animator.SetBool("IsRunning", true); // Run

        // panel que activa shift para correr!!
        if (panelFadeScript != null)
            panelFadeScript.SetActive(true);

        if (flashlightFlicker != null)
            flashlightFlicker.StartFlicker();

        Debug.Log($"[{Time.time:F2}] IntroDone");
        IntroDone = true;
        yield return new WaitForSeconds(0.5f);
        Run();

        contactEnabled = true; // recién acá el contacto puede matarte
    }

    public void Run()
    {
        Debug.Log($"[{Time.time:F2}] Run llamado");

        if (staticImage != null)
            staticImage.SetActive(true);

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Run: no hay NavMesh a menos de 5 m del enemigo");
            return;
        }

        transform.position = hit.position; // lo apoya sobre el NavMesh
        agent.enabled = true;              // recién ahora se enciende el agente
        agent.speed = runSpeed;
        agent.isStopped = false;
        run = true;
    }

    // ---------- FASE 2: Contacto / muerte ----------

    public void OnCatchContact()
    {
        if (!contactEnabled || jumpscareActive) return;

        jumpscareActive = true;
        StartDeathSequence();
    }

    private void StartDeathSequence()
    {
        Debug.Log("StartDeathSequence llamado");
        run = false;
        introMoving = false;

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        animator.applyRootMotion = false;
        animator.SetBool("IsRunning", false);
        animator.CrossFade(endStateName, endCrossFade);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (cameraController != null && viewPoint != null)
            cameraController.LookAtViewpoint(viewPoint, cameraLookSpeed);

        if (playerDeathAnimator != null)
            playerDeathAnimator.SetTrigger("play");
        if (shakeTarget != null && viewPoint != null)
            StartCoroutine(CameraEatRoutine());

        StartCoroutine(DeathRoutine());
    }

    // Se llama desde OnScreamEnd cuando el Scream es el de la muerte
    public void OnEndAnimationFinished() => endAnimFinished = true;

    // Compatibilidad: si OnScreamEnd todavía llega durante la muerte
    public void OnDeathScreamEnd() => endAnimFinished = true;

    private IEnumerator DeathRoutine()
    {
        if (waitForEndEvent)
            yield return new WaitUntil(() => endAnimFinished);
        else
            yield return new WaitForSeconds(endAnimationDelay);

        // Fade a negro
        if (deathFadeGroup != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                deathFadeGroup.alpha = t / fadeDuration;
                yield return null;
            }
            deathFadeGroup.alpha = 1f;
        }

        // Música justo cuando queda todo negro
        if (blackScreenSource != null && blackScreenMusic != null)
        {
            blackScreenSource.clip = blackScreenMusic;
            blackScreenSource.Play();
        }

        yield return new WaitForSeconds(blackHoldTime);

        if (cameraController != null)
            cameraController.StopLook();

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene(deathSceneName);
    }

    public void OnEndLunge() => lungeEvent = true;

    private Vector3 ShakeOffset(float elapsed)
    {
        if (elapsed >= shakeDuration) return Vector3.zero;
        return Random.insideUnitSphere * shakeMagnitude * (1f - elapsed / shakeDuration);
    }

    private IEnumerator CameraEatRoutine()
    {
        Transform cam = shakeTarget;
        Vector3 startPos = cam.position;

        Vector3 away = startPos - viewPoint.position;
        away.y = 0f;
        away.Normalize();

        Vector3 backPos = startPos + away * pullBackDistance;
        Vector3 lungePos = viewPoint.position + away * lungeStopDistance;
        lungePos.y = startPos.y;

        float elapsed = 0f;

        // 1) Se aleja
        for (float t = 0; t < pullBackTime; t += Time.deltaTime)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / pullBackTime);
            cam.position = Vector3.Lerp(startPos, backPos, k) + ShakeOffset(elapsed);
            yield return null;
        }

        // 2) Espera (temblando) hasta el evento o hasta el delay
        while (!lungeEvent && elapsed < lungeStartDelay)
        {
            elapsed += Time.deltaTime;
            cam.position = backPos + ShakeOffset(elapsed);
            yield return null;
        }

        // 3) Se lanza hacia adelante
        for (float t = 0; t < lungeTime; t += Time.deltaTime)
        {
            elapsed += Time.deltaTime;
            float k = (t / lungeTime) * (t / lungeTime); // acelera
            cam.position = Vector3.Lerp(backPos, lungePos, k) + ShakeOffset(elapsed);
            yield return null;
        }
        cam.position = lungePos;
    }
    public void ApplyScreamScale()
    {
        SetScale(screamScale);
    }

    // Vuelve a la escala original (se puede llamar también desde un evento)
    public void ResetScale()
    {
        SetScale(originalScale);
    }

    private void SetScale(Vector3 target)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        if (scaleTransitionTime <= 0f)
            transform.localScale = target;
        else
            scaleRoutine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < scaleTransitionTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / scaleTransitionTime);
            yield return null;
        }

        transform.localScale = target;
    }
}