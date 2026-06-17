using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform[] waypoints;
    public float moveSpeed = 2.5f;
    public float waypointTolerance = 0.6f;
    public float waitAtWaypoint = 1f;

    [Header("El FOV")]
    public float detectionRange = 10f;
    public float hearDetectionRange = 4f;

    [Range(0f, 180f)]
    public float fieldOfViewAngle = 90f;
    public LayerMask obstacles;

    [Header("Referencias de Eventos")]
    public PlayerSwitcher switcher;
    public MinigamesManager minigamesManager;
    [SerializeField] private Transform spawnPoint;

    private bool playerDead = false;
    private int nowWaypoint = 0;
    private float waitTimer = 0f;
    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool minigameActive = false;
    private float resetCooldown = 0f;
    private const float resetCooldownTime = 5f;
    private MonsterAudioController audioController;
    private bool wasChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.stoppingDistance = waypointTolerance;
        }

        audioController = GetComponent<MonsterAudioController>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        if (waypoints == null || waypoints.Length == 0)
            waypoints = new Transform[] { transform };

        if (minigamesManager != null)
        {
            minigamesManager = minigamesManager.GetComponent<MinigamesManager>();
        }

        if (switcher == null)
        {
            switcher = Object.FindFirstObjectByType<PlayerSwitcher>();
        }
    }

    void Update()
    {
        if (playerDead) { return; }

        if (resetCooldown > 0f)
        {
            resetCooldown -= Time.deltaTime;
            PatrolBehaviour();
            return;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                player = p.transform;
            }
            else
            {
                PatrolBehaviour();
                return;
            }
        }

        bool canSee = CanSeePlayer();
        bool canHear = CanHearPlayerNearby();
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        bool shouldChase = !minigameActive && (canSee || canHear || (distToPlayer <= 5f && !isAttacking));

        if (audioController != null)
        {
            if (shouldChase && !wasChasing)
            {
                audioController.PlayRoar();
                GameMusicManager.Instance.SetCombatState(true);
            }
            else if (!shouldChase && wasChasing)
            {
                GameMusicManager.Instance.SetCombatState(false);
            }
        }

        wasChasing = shouldChase;

        if (shouldChase)
        {
            if (distToPlayer <= 2.5f && !isAttacking)
            {
                if (audioController != null)
                    audioController.PlayAttackSound();

                isAttacking = true;

                if (agent != null)
                {
                    agent.ResetPath();
                }

                Debug.Log("Dron interceptado. Iniciando minijuego...");
                Die();
                return;
            }

            if (!isAttacking && !minigameActive)
            {
                MoveTowards(player.position);
                return;
            }
        }

        PatrolBehaviour();
    }

    void PatrolBehaviour()
    {
        if (waypoints.Length == 0)
            return;

        Transform wp = waypoints[nowWaypoint];

        float dist = Vector3.Distance(transform.position, wp.position);

        if (dist <= waypointTolerance)
        {
            if (waitTimer <= 0f)
            {
                waitTimer = waitAtWaypoint;

                if (agent != null)
                {
                    agent.ResetPath();
                }
            }
            else
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    nowWaypoint = (nowWaypoint + 1) % waypoints.Length;
                }
            }
        }
        else
        {
            MoveTowards(wp.position);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle > fieldOfViewAngle * 0.5f)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = (player.position + Vector3.up * 1f - origin).normalized;

        if (Physics.Raycast(origin, direction, distance, obstacles))
        {
            return false;
        }

        return true;
    }

    void MoveTowards(Vector3 target)
    {
        if (agent == null)
            return;

        agent.speed = moveSpeed;
        agent.SetDestination(target);

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            FaceTarget(transform.position + agent.velocity);
        }
    }

    bool CanHearPlayerNearby()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= hearDetectionRange;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void Die()
    {
        if (playerDead) return;
        Debug.Log("Dron interceptado. Iniciando secuencia de reparacion...");
        TriggerDroneFailure();
    }

    void TriggerDroneFailure()
    {
        if (minigamesManager != null)
        {
            minigameActive = true;
            minigamesManager.DronFailure();
        }
    }

    public void ResetEnemy()
    {
        isAttacking = false;
        minigameActive = false;
        resetCooldown = resetCooldownTime;

        wasChasing = false;
    }

    public void RestartPatrol()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.Warp(spawnPoint.position);
        }

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        nowWaypoint = 0;
        waitTimer = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = new Color(1f, 0.9f, 0f, 0.4f);
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                Gizmos.DrawSphere(waypoints[i].position, 0.12f);

                int next = (i + 1) % waypoints.Length;

                if (waypoints[next] != null)
                {
                    Gizmos.DrawLine(
                        waypoints[i].position,
                        waypoints[next].position
                    );
                }
            }
        }
    }
}