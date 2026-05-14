using UnityEngine;
using UnityEngine.UI;
public class CrosshairRaycast : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;

    [Header("Crosshair")]
    [SerializeField] Image crosshairImage;

    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite interactSprite;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, rayDistance, layerMask))
        {
            Debug.Log("colisionamos con: " + hit.collider.gameObject.name);
            crosshairImage.sprite = interactSprite;
            Debug.DrawLine(origin, hit.point, Color.red);
        }
        else
        {
            crosshairImage.sprite = defaultSprite;
            Debug.DrawLine(origin, origin + direction * rayDistance, Color.green);
        }
    }
}
