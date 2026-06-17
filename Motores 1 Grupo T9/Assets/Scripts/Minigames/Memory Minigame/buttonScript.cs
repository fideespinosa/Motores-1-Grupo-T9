using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] private int buttonNumber;
    [SerializeField] private PanelManager panelManager;

    [Header("Visual")]
    [SerializeField] private float pressDistance = 0.02f;
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color pressedColor = Color.green;

    private Vector3 originalPosition;
    private Color originalColor;

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
        transform.localPosition = originalPosition;
        buttonRenderer.material.color = originalColor;
    }
}