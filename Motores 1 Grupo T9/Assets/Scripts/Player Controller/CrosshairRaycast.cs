using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CrosshairRaycast : MonoBehaviour
{
    [SerializeField] ScreensManagerScript screensManager;

    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;

    [SerializeField] MinigamesManager minigamesManager;

    [Header("Crosshair")]
    [SerializeField] Image crosshairImage;

    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite interactSprite;

    [Header("Player Inventory")]
    [SerializeField] PlayerInventory playerInventory;


    bool screen1Opened = false;

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
            Debug.DrawLine(origin, hit.point, Color.red);
            crosshairImage.sprite = interactSprite;

            
            if (Input.GetMouseButtonDown(0))
            {
                screen1Opened = screensManager.Screen1Opened();

                if (hit.collider.gameObject.CompareTag("Screen 1") && minigamesManager.isAlarmActive)
                {
                    screensManager.OpenPanelScreen1();
                }

                if (hit.collider.gameObject.CompareTag("NextLvl"))
                {
                    playerInventory.CheckVictory(); 
                }
            }
        }
        else
        {
            crosshairImage.sprite = defaultSprite;
            //Debug.Log("DEFAULT");
            Debug.DrawRay(origin, direction * rayDistance, Color.green);
        }
    }
}
