using UnityEngine;

public class Crosshairdron : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] EnemyBehaviorLVL2 enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy = enemy.GetComponent<EnemyBehaviorLVL2>();
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


                if (hit.collider.gameObject.CompareTag("EnemyLVL2"))
                {
                enemy.StartScreaming();
                }

        }
        else
        {
            Debug.DrawRay(origin, direction * rayDistance, Color.green);
        }
    }
}
