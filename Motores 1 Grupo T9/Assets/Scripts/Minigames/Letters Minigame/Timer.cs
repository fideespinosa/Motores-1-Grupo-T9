using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timeLeft = 10f;

    float initialTime;
    bool isRunning = true;

    [SerializeField] KeyQueueManager keyManager;

    void Start()
    {
        initialTime = timeLeft;
    }

    void Update()
    {
        if (!isRunning) return;

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeLeft).ToString();
        }
        else
        {
            timerText.text = "0";
            isRunning = false;
            keyManager.LoseGame();
        }
    }

    public void ResetTimer()
    {
        timeLeft = initialTime;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}