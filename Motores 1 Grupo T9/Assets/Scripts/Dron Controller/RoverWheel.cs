using UnityEngine;

public class RoverWheel : MonoBehaviour
{
    public Rigidbody rb;
    public float restDistance = 0.5f;
    public float springStrength = 500f;
    public float springDamper = 30f;
    public LayerMask groundLayer;

    private bool isGrounded = false;
    public bool IsGrounded => isGrounded; 

    void FixedUpdate()
    {
        
        Vector3 origin = transform.position + (transform.up * 0.4f);
        float rayDistance = restDistance + 0.6f;

        if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, rayDistance, groundLayer))
        {
            isGrounded = true;
            Debug.DrawLine(origin, hit.point, Color.green);

            Vector3 springDir = transform.up;
            Vector3 tireWorldVel = rb.GetPointVelocity(transform.position);

            float actualHitDistance = hit.distance - 0.4f;
            float offset = restDistance - actualHitDistance;

           
            offset = Mathf.Clamp(offset, -restDistance, restDistance);

            float vel = Vector3.Dot(springDir, tireWorldVel);
            float force = (offset * springStrength) - (vel * springDamper);

            
            force = Mathf.Clamp(force, -2000f, 2000f);

            rb.AddForceAtPosition(springDir * force, transform.position, ForceMode.Force);
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(origin, -transform.up * rayDistance, Color.red);
        }
    }

    public void ApplyDriveForce(float force)
    {
        
        if (isGrounded)
        {
            rb.AddForceAtPosition(rb.transform.forward * force, transform.position, ForceMode.Force);
        }
    }
}