using System.Collections;
using UnityEngine;

public class CameraSequenceController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // la Camera del Player
    [SerializeField] private MonoBehaviour playerMovementScript; // PlayerMovement, para bloquear el mouse look durante la cinemática

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Transform originalParent;

    private void Awake()
    {
        if (cameraTransform != null)
        {
            originalParent = cameraTransform.parent;
            originalLocalPosition = cameraTransform.localPosition;
            originalLocalRotation = cameraTransform.localRotation;
        }
    }

    public void MoveToViewpoint(Transform viewPoint, float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToViewpointRoutine(viewPoint, duration));
    }

    public void ReturnToPlayer(float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(ReturnToPlayerRoutine(duration));
    }

    public void LookAtViewpoint(Transform viewPoint, float speed = 5f)
    {
        StopAllCoroutines();
        StartCoroutine(LookAtViewpointRoutine(viewPoint, speed));
    }

    public void StopLook()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveToViewpointRoutine(Transform viewPoint, float duration)
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cameraTransform.position = Vector3.Lerp(startPos, viewPoint.position, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, viewPoint.rotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = viewPoint.position;
        cameraTransform.rotation = viewPoint.rotation;
    }

    private IEnumerator ReturnToPlayerRoutine(float duration)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 targetPos = originalParent.TransformPoint(originalLocalPosition);
        Quaternion targetRot = originalParent.rotation * originalLocalRotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalLocalPosition;
        cameraTransform.localRotation = originalLocalRotation;

        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }

    private IEnumerator LookAtViewpointRoutine(Transform viewPoint, float speed)
    {
        while (true)
        {
            Vector3 direction = (viewPoint.position - cameraTransform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }
    }
}