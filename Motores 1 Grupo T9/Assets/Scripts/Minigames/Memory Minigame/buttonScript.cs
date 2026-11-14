using System.Collections;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] private int buttonNumber;
    [SerializeField] private PanelManager panelManager;

    [Header("Visual")]
    [SerializeField] private float pressDistance = 0.02f;
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color pressedColor = Color.green;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.04f;
    [SerializeField] private Color errorColor = Color.red;

    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine shakeRoutine;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalColor = buttonRenderer.material.color;
    }

    public void Press()
    {
        panelManager.PressButton(this);
    }

    public int GetButtonNumber()
    {
        return buttonNumber;
    }

    public void SetPressed()
    {
        transform.localPosition = originalPosition + Vector3.back * pressDistance;
        buttonRenderer.material.color = pressedColor;
    }

    public void ResetVisual()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        transform.localPosition = originalPosition;
        buttonRenderer.material.color = originalColor;
    }

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;
        buttonRenderer.material.color = errorColor;

        while (elapsed < shakeDuration)
        {
            float damper = 1f - (elapsed / shakeDuration); // se va calmando
            float x = Random.Range(-1f, 1f) * shakeStrength * damper;
            float y = Random.Range(-1f, 1f) * shakeStrength * damper;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        buttonRenderer.material.color = originalColor;
        shakeRoutine = null;
    }
}