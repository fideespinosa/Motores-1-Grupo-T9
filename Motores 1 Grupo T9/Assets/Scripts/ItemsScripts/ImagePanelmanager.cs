using UnityEngine;
using UnityEngine.UI;

public class ImagePanelManager : MonoBehaviour
{
    public static ImagePanelManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image displayedImage;

    [Header("Player Reference")]
    [Tooltip("Se desactiva mientras se muestra la imagen para que la cámara quede quieta.")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Settings")]
    [Tooltip("Si está activo, el cursor se libera mientras se ve la imagen (útil si normalmente está bloqueado).")]
    [SerializeField] private bool unlockCursorWhenShown = true;

    public bool IsShowingImage { get; private set; }

    private bool playerMovementWasEnabledBeforeShow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsShowingImage) return;

        if (Input.GetMouseButtonDown(0))
        {
            HideImage();
        }
    }

    public void ShowImage(Sprite sprite)
    {
        if (panel == null || displayedImage == null || sprite == null) return;
        if (IsShowingImage) return;

        displayedImage.sprite = sprite;
        panel.SetActive(true);
        IsShowingImage = true;

        if (playerMovement != null)
        {
            playerMovementWasEnabledBeforeShow = playerMovement.enabled;
            playerMovement.enabled = false;
        }

        if (unlockCursorWhenShown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HideImage()
    {
        if (panel == null) return;

        panel.SetActive(false);
        IsShowingImage = false;

        if (playerMovement != null && playerMovementWasEnabledBeforeShow)
        {
            playerMovement.enabled = true;
        }

        if (unlockCursorWhenShown)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}