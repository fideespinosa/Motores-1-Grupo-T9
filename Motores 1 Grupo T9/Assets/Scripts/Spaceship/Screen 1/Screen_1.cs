using UnityEngine;

public class Screen_1 : MonoBehaviour
{
    [SerializeField] ScreensManagerScript screensManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screensManager = screensManager.GetComponent<ScreensManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CanvasOff()
    {
        gameObject.SetActive(false);
    }

}
