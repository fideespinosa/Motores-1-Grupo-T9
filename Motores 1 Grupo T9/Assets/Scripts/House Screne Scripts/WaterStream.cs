
using UnityEngine;

public class WaterStream : MonoBehaviour
{
    public Transform faucet;
    public Transform impactPoint;

    public float openingSpeed = 5f;
    public float flowSpeed = 1.5f;

    private Renderer waterRenderer;
    private Vector3 initialScale;
    private float currentLength;
    private float targetLength;
    private Vector2 textureOffset;

    void Start()
    {
        waterRenderer = GetComponent<Renderer>();
        initialScale = transform.localScale;
        transform.localScale = new Vector3(initialScale.x, 0f, initialScale.z);

    }

    void Update()
    {
        currentLength = Mathf.MoveTowards(
            currentLength,
            targetLength,
            openingSpeed * Time.deltaTime
        );

        UpdateWater();

        if (currentLength > 0.01f && waterRenderer != null)
        {
            textureOffset.y -= flowSpeed * Time.deltaTime;
            waterRenderer.material.mainTextureOffset = textureOffset;
        }
    }

    public void Open()
    {
        targetLength = Vector3.Distance(faucet.position, impactPoint.position);
    }

    public void Close()
    {
        targetLength = 0f;
    }

    void UpdateWater()
    {
        Vector3 direction = (faucet.position - impactPoint.position).normalized;

        transform.position = faucet.position - direction * (currentLength / 2f);

        transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

        transform.localScale = new Vector3(
            initialScale.x,
            currentLength / 2f,
            initialScale.z
        );
    }
}
