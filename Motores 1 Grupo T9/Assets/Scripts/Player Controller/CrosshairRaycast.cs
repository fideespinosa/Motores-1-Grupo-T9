using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CrosshairRaycast : MonoBehaviour
{
    [SerializeField] ScreensManagerScript screensManager;

    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;

    [Header("Crosshair")]
    [SerializeField] Image crosshairImage;

    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite interactSprite;


    private void Start()
    {
        screensManager = screensManager.GetComponent<ScreensManagerScript>();
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, rayDistance, layerMask))
        {
            crosshairImage.sprite = interactSprite;

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.CompareTag("Screen 1"))
                {
                    screensManager.OpenPanelScreen1();
                }
            }
        }
        else
        {
            crosshairImage.sprite = defaultSprite;
        }
    }
}
