using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject minigameCanvas; 

    private bool playerDead = false;
    private int nowWaypoint = 0;
    private float waitTimer = 0f;
    private Transform player;
    private Rigidbody rb;
    private bool isAttacking = false; 
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p) player = p.transform;
        if (waypoints == null || waypoints.Length == 0)
            waypoints = new Transform[] { transform };
    }

    void Update()
    {

        if (playerDead) { return; }


        if (CanSeePlayer() || CanHearPlayerNearby())
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            
            if (distToPlayer <= 2f && !isAttacking)
            {
                isAttacking = true; 
                Debug.Log("Dron interceptado. Iniciando minijuego...");
                Die();
                return; 
            }

            if (!isAttacking)
            {
                MoveTowards(player.position);
            }
            return;
        }

        if (waypoints.Length == 0) { return; }

        Transform wp = waypoints[nowWaypoint];
        float dist = Vector3.Distance(transform.position, wp.position);

        if (dist <= waypointTolerance)
        {
            if (waitTimer <= 0f)
                waitTimer = waitAtWaypoint;
            else
            {
                waitTimer -= Time.deltaTime;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                if (waitTimer <= 0f) 
                    nowWaypoint = (nowWaypoint + 1) % waypoints.Length;
            }
        }
        else
        {
            MoveTowards(wp.position);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) { return false; }

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > detectionRange) { return false; }

        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle > fieldOfViewAngle * 0.5f) { return false; }

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
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        Vector3 rayOrigin = transform.position + Vector3.up * 1.2f;

        bool obstacleFront = Physics.Raycast(rayOrigin, transform.forward, 1.0f, obstacles);
        bool obstacleLeft = Physics.Raycast(rayOrigin, Quaternion.Euler(0, -30, 0) * transform.forward, 1.0f, obstacles);
        bool obstacleRight = Physics.Raycast(rayOrigin, Quaternion.Euler(0, 30, 0) * transform.forward, 1.0f, obstacles);

        if (obstacleFront)
        {
            if (!obstacleLeft) { dir = Quaternion.Euler(0, -30, 0) * dir; }
            else if (!obstacleRight) { dir = Quaternion.Euler(0, 30, 0) * dir; }
            else { dir = Quaternion.Euler(0, 180, 0) * dir; }
        }

        Vector3 velocity = dir.normalized * moveSpeed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        FaceTarget(transform.position + dir);
    }

    bool CanHearPlayerNearby()
    {
        if (player == null) { return false; }

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

        
        Debug.Log("Dron interceptado. Iniciando secuencia de reparación...");

        TriggerDroneFailure();
    }

    void TriggerDroneFailure()
    {
        
        if (switcher != null)
        {
            switcher.SetControl(false); 
            switcher.enabled = false;   
        }

      
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
            
        }

        rb.linearVelocity = Vector3.zero;
        nowWaypoint = (nowWaypoint + 1) % waypoints.Length;
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }
    public void ResetEnemy()
    {
        isAttacking = false;
        
        nowWaypoint = (nowWaypoint + 1) % waypoints.Length;
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
                    Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}