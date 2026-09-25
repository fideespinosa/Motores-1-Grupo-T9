using UnityEngine;

public class LadderViewpoint : MonoBehaviour, IInteractable
{
    [Header("Viewpoint")]
    [Tooltip("Empty GameObject posicionado y rotado donde la cámara debe pararse.")]
    [SerializeField] private Transform viewpoint;

    [Header("Player References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform playerCameraTransform;

    [Header("Look Restriction")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minYaw = -45f;
    [SerializeField] private float maxYaw = 45f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 30f;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool isActive = false;
    private Vector3 storedLocalPosition;
    private Quaternion storedLocalRotation;
    private float currentYaw;
    private float currentPitch;

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void Update()
    {
        if (!isActive) return;
        if (ImagePanelManager.Instance != null && ImagePanelManager.Instance.IsShowingImage) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToPlayer();
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw = Mathf.Clamp(currentYaw + mouseX, minYaw, maxYaw);
        currentPitch = Mathf.Clamp(currentPitch - mouseY, minPitch, maxPitch);

        playerCameraTransform.rotation = viewpoint.rotation * Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    public void Action()
    {
        if (isActive) return;
        if (viewpoint == null || playerCameraTransform == null) return;

        storedLocalPosition = playerCameraTransform.localPosition;
        storedLocalRotation = playerCameraTransform.localRotation;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        playerCameraTransform.position = viewpoint.position;
        playerCameraTransform.rotation = viewpoint.rotation;

        currentYaw = 0f;
        currentPitch = 0f;

        isActive = true;
    }

    private void ReturnToPlayer()
    {
        isActive = false;

        playerCameraTransform.localPosition = storedLocalPosition;
        playerCameraTransform.localRotation = storedLocalRotation;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    public void OnHoverEnter()
    {
        if (isActive) return;

        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}