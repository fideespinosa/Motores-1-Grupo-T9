using UnityEngine;

public class RoverWheel : MonoBehaviour
{
    public Rigidbody rb;
    public float restDistance = 0.5f; 
    public float springStrength = 500f;
    public float springDamper = 20f; 

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, restDistance + 0.2f))
        {
            
            Vector3 springDir = transform.up;

            
            Vector3 tireWorldVel = rb.GetPointVelocity(transform.position);
            float offset = restDistance - hit.distance;
            float vel = Vector3.Dot(springDir, tireWorldVel);

           
            float force = (offset * springStrength) - (vel * springDamper);

            rb.AddForceAtPosition(springDir * force, transform.position);
        }
    }

    public void ApplyDriveForce(float force)
    {
        
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, restDistance + 0.2f))
        {
            rb.AddForceAtPosition(rb.transform.forward * force, transform.position);
        }
    }
}