using UnityEngine;
using UnityEngine.AI;

public class EnemyWallEntry : MonoBehaviour
{
    private Vector3 entryPoint;
    private float speed = 3f;

    private NavMeshAgent agent;
    private EnemyMovement enemyMovement;
    private Rigidbody rb;
    private Collider col;

    public void Init(Vector3 navMeshEntryPoint, float crossSpeed = 3f)
    {
        entryPoint = navMeshEntryPoint;
        speed = crossSpeed;

        agent = GetComponent<NavMeshAgent>();
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (agent != null) agent.enabled = false;
        if (enemyMovement != null) enemyMovement.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (col != null)
        {
            col.enabled = false;
        }
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, entryPoint, speed * Time.deltaTime);

        Vector3 dir = entryPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        float distXZ = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(entryPoint.x, 0, entryPoint.z)
        );

        Debug.Log($"[EnemyWallEntry] distXZ al entryPoint: {distXZ}"); // TEMPORAL

        if (distXZ < 0.15f)
        {
            transform.position = entryPoint;

            if (rb != null) rb.isKinematic = false;
            if (col != null) col.enabled = true;

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(entryPoint);
            }

            if (enemyMovement != null)
                enemyMovement.enabled = true;

            Destroy(this);
        }
    }
}