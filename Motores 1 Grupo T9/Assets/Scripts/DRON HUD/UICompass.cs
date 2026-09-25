using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassUI : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float pixelsPerDegree = 2f;

    private List<RectTransform> markers = new();

    private float previousYaw;

    private float leftLimit = -600f;
    private float rightLimit = 600f;
    private float wrapDistance = 1200f;

    private void Start()
    {
        foreach (Transform child in transform)
            markers.Add(child.GetComponent<RectTransform>());

        previousYaw = playerCamera.eulerAngles.y;
    }

    private void LateUpdate()
    {
        float yaw = playerCamera.eulerAngles.y;

        float delta = Mathf.DeltaAngle(previousYaw, yaw);

        foreach (var marker in markers)
        {
            marker.anchoredPosition -= new Vector2(delta * pixelsPerDegree, 0);

            if (marker.anchoredPosition.x > rightLimit)
                marker.anchoredPosition -= new Vector2(wrapDistance, 0);

            else if (marker.anchoredPosition.x < leftLimit)
                marker.anchoredPosition += new Vector2(wrapDistance, 0);
        }

        previousYaw = yaw;
    }
}