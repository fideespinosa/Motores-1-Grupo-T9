using UnityEngine;

public class DroneColliderToggle : MonoBehaviour
{
    private Collider[] colliders;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true); // true = incluye hijos desactivados
        Debug.Log("DroneColliderToggle: " + colliders.Length + " colliders en " + name);
    }

    public void SetColliders(bool state)
    {
        Debug.Log("SetColliders(" + state + ")");
        foreach (var c in colliders)
            c.enabled = state;
    }
}