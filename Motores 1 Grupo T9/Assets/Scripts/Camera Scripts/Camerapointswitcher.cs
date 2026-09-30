using System.Collections;
using UnityEngine;

public class CameraPointSwitcher : MonoBehaviour
{
    private enum StartPoint
    {
        PointA,
        PointB
    }

    [Header("Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private StartPoint startingPoint = StartPoint.PointA;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.4f;

    [Header("Escape Toggle")]
    [SerializeField] private bool enableEscapeToggle = false;

    [Header("References")]
    [SerializeField] private MonoBehaviour fpsOldInput;

    private bool isAtPointA;
    private bool isMoving = false;

    private void Start()
    {
        isAtPointA = startingPoint == StartPoint.PointA;

        Transform target = isAtPointA ? pointA : pointB;
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        if (fpsOldInput != null)
        {
            fpsOldInput.enabled = isAtPointA;
        }
    }

    public void SwitchCamera()
    {
        if (!enableEscapeToggle) return;
        if (isMoving) return;

        StartCoroutine(MoveTo(pointB, false));

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
        {
            if (!isAtPointA)
            {
                StartCoroutine(MoveTo(pointA, true));
            }

        }
        
    }

    private IEnumerator MoveTo(Transform target, bool willBeAtPointA)
    {
        if (target == null) yield break;

        isMoving = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        isAtPointA = willBeAtPointA;
        isMoving = false;

        if (fpsOldInput != null)
        {
            fpsOldInput.enabled = isAtPointA;
        }
    }
}