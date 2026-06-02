using UnityEngine;

public class Crosshairdron : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] CinematicManager cinematicManager;
    bool enemyDetected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinematicManager = cinematicManager.GetComponent<CinematicManager>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, rayDistance, layerMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);


                if (hit.collider.gameObject.CompareTag("EnemyLVL2") && !enemyDetected)
                {
                 enemyDetected = true;
                 cinematicManager.StartMonsterCinematic();
                }

        }
        else
        {
            Debug.DrawRay(origin, direction * rayDistance, Color.green);
        }
    }
}
