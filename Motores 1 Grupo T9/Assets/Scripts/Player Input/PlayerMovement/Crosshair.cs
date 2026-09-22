using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Crosshair : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;
    [Header("Crosshair")]
    [SerializeField] Image crosshairImage;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite interactSprite;

    private IInteractable currentHoveredInteractable;

    void Start()
    {

    }
    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        if (Physics.Raycast(origin, direction, out hit, rayDistance, layerMask))
        {
            Debug.DrawLine(origin, hit.point, Color.red);

            IInteractable interactable = null;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
            {
                interactable = hit.collider.GetComponent<IInteractable>();
                crosshairImage.sprite = interactSprite;
            }
            else
            {
                crosshairImage.sprite = defaultSprite;
            }

                UpdateHover(interactable);

            if (interactable != null && Input.GetMouseButtonDown(0))
            {
                interactable.Action();
            }
        }
        else
        {
            crosshairImage.sprite = defaultSprite;
            Debug.DrawRay(origin, direction * rayDistance, Color.green);
            UpdateHover(null);
        }
    }

    private void UpdateHover(IInteractable newInteractable)
    {
        if (newInteractable == currentHoveredInteractable) return;

        if (currentHoveredInteractable != null)
        {
            currentHoveredInteractable.OnHoverExit();
        }

        if (newInteractable != null)
        {
            newInteractable.OnHoverEnter();
        }

        currentHoveredInteractable = newInteractable;
    }
}