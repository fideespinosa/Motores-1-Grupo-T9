using UnityEngine;

public class MemoryInstructions : MonoBehaviour
{
    [SerializeField] private GameObject zone;

    private void OnEnable()
    {
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(zone);
        gameObject.SetActive(false);
    }
}
