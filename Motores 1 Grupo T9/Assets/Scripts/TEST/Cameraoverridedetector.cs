using UnityEngine;

public class CameraOverrideDetector : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        ReportSuspects();
    }

    private void ReportSuspects()
    {
        Debug.Log("=== CameraOverrideDetector: componentes en este objeto ===");

        Component[] components = GetComponents<Component>();
        foreach (Component c in components)
        {
            if (c == null) continue;
            Debug.Log("- " + c.GetType().Name);
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            Debug.LogWarning("Hay un Animator en este objeto. Apply Root Motion: " + animator.applyRootMotion + " | Enabled: " + animator.enabled);
        }

        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script == null) continue;
            string typeName = script.GetType().Name;
            if (typeName.Contains("Cinemachine") || typeName.Contains("Brain") || typeName.Contains("VirtualCamera"))
            {
                Debug.LogWarning("Sospechoso de Cinemachine encontrado: " + typeName + " | Enabled: " + script.enabled);
            }
        }
    }

    private void LateUpdate()
    {
        if (transform.position != lastPosition)
        {
            Debug.Log("Posición cambió este frame de " + lastPosition + " a " + transform.position);
        }

        if (transform.rotation != lastRotation)
        {
            Debug.Log("Rotación cambió este frame de " + lastRotation.eulerAngles + " a " + transform.rotation.eulerAngles);
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}