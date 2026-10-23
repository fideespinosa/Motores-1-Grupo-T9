
using UnityEngine;

public class CameraLookAtPoint : MonoBehaviour
{
    [Header("Look Target")]
    [SerializeField] private Transform lookTarget;

    [Header("Player References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private FPS_OldInput fpsOldInput;

    public void LookAtTarget()
    {
        if (lookTarget == null || playerTransform == null) return;

        Transform cameraTransform = playerCameraTransform;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform == null) return;

        Vector3 direction = lookTarget.position - playerTransform.position;
        Vector3 horizontalDirection = new Vector3(direction.x, 0f, direction.z);

        if (horizontalDirection.sqrMagnitude > 0.0001f)
            playerTransform.rotation = Quaternion.LookRotation(horizontalDirection);

        Vector3 cameraDirection = lookTarget.position - cameraTransform.position;

        if (cameraDirection.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);

        if (fpsOldInput != null)
            fpsOldInput.SetLookRotation(targetRotation);
        else
            cameraTransform.rotation = targetRotation;
    }
}
