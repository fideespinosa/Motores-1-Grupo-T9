using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 direction = Vector3.up;
    [SerializeField, Min(0f)] private float amplitude = 0.25f;
    [SerializeField, Min(0.1f)] private float cycleDuration = 1.6f;

    [Header("Variation")]
    [SerializeField] private bool randomizeStartPhase = false;

    private Vector3 startLocalPosition;
    private float phaseOffset;
    private float elapsed;

    private void OnEnable()
    {
        startLocalPosition = transform.localPosition;
        elapsed = 0f;
        phaseOffset = randomizeStartPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void OnDisable()
    {
        transform.localPosition = startLocalPosition;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        float angle = elapsed * (Mathf.PI * 2f / cycleDuration) + phaseOffset;
        float offset = Mathf.Sin(angle) * amplitude;

        transform.localPosition = startLocalPosition + direction.normalized * offset;
    }
}