using UnityEngine;

public class DroneColliderToggle : MonoBehaviour
{
    private Collider[] colliders;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
    }

    public void SetColliders(bool state)
    {
        foreach (var c in colliders)
            c.enabled = state;
    }
}