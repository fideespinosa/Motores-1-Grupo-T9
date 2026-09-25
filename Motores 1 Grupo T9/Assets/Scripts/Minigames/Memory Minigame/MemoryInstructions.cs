using UnityEngine;

public class MemoryInstructions : MonoBehaviour
{
    [SerializeField] private GameObject zone;
    [SerializeField] private MinigamesManager minigamesManager;
    private void OnEnable()
    {
        minigamesManager.FreezeGame();
        Time.timeScale = 0f;
        Destroy(zone);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        gameObject.SetActive(false);
        minigamesManager.UnfreezeGame();
    }
}
