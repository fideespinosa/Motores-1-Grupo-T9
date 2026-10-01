using UnityEngine;

public class MemoryInstructions : MonoBehaviour
{
    [SerializeField] private GameObject zone;
    [SerializeField] private MinigamesManager minigamesManager;
    [SerializeField] private PanelManager panelManager;
    private void OnEnable()
    {
        minigamesManager.FreezeGame();
        Time.timeScale = 0f;
        Destroy(zone);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        panelManager.StartMemoryMinigame();
        gameObject.SetActive(false);
        minigamesManager.UnfreezeGame();
    }
}
