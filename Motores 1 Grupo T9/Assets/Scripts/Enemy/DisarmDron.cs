using UnityEngine;

public class DisarmDron : MonoBehaviour
{
    [Header("Detección")]
    public float hearDetectionRange = 10f;

    [Header("Rotación")]
    public float rotationSpeed = 5f;
    public float facingThreshold = 5f;

    [Header("Referencias de Eventos")]
    public PlayerSwitcher switcher;
    public MinigamesManager minigamesManager;
    public GameObject alarmScreen;
    public GameObject alarmLight;

    private bool minigameActive = false;
    private Transform player;

    void Start()
    {

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p)
        {
            Debug.Log($"Encuentra el player");
            player = p.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con tag 'Player'!");
        }

        minigamesManager = minigamesManager.GetComponent<MinigamesManager>();
    }

    void Update()
    {
        if (minigameActive) return;

        // Debug.Log($"Lo escucha: {CanHearPlayerNearby()}");

        if (CanHearPlayerNearby())
        {
            
            FaceTarget(player.position);

            if (IsFacingTarget(player.position, facingThreshold))
            {
                TriggerDroneFailure();
            }
        }
    }


    bool CanHearPlayerNearby()
    {
        if (player == null) { return false; }

        float distance = Vector3.Distance(transform.position, player.position);

        // Debug.Log($"Distancia {distance}");

        return distance <= hearDetectionRange;
    }

    bool IsFacingTarget(Vector3 target, float threshold)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        return Vector3.Angle(transform.forward, dir) < threshold;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void TriggerDroneFailure()
    {
        if (minigamesManager != null)
        {
            minigameActive = true;
            minigamesManager.DronFailure();
            //alarmScreen.SetActive(true);
            //alarmLight.SetActive(true);

            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, hearDetectionRange);
    }
}