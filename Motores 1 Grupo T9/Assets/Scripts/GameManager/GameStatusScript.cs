using UnityEngine;

public class GameStatusScript : MonoBehaviour
{
    public static GameStatusScript Instance;

    [Header("States")]
    public bool minigameRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void StartMinigame()
    {
        minigameRunning = true;
    }

    public void EndMinigame()
    {
        minigameRunning = false;
    }

    public bool IsMinigameRunning()
    {
        return minigameRunning;
    }
}
