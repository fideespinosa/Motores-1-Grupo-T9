using UnityEngine;

public class DroneMagnet : MonoBehaviour
{
    private Rigidbody rb;

    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float magnetForce = 8f; 
    [SerializeField] private float maxDistance = 1.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, maxDistance, groundLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.blue);

            
            Vector3 downforceDirection = -hit.normal;

            
            rb.AddForce(downforceDirection * magnetForce, ForceMode.Acceleration);
        }
    }
}