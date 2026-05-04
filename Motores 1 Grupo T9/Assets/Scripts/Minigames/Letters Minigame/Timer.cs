using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float Totaltime = 15f;
    float initialTime;

    private float timeLeft;
    private bool isRunning = false; 
    [SerializeField] KeyQueueManager keyManager;

    private void Start()
    {
        initialTime = Totaltime;
    }
    void Update()
    {
        if (!isRunning) return;

        if (timeLeft > 0)
        {
            timeLeft -= Time.unscaledDeltaTime;
            timerText.text = Mathf.Ceil(timeLeft).ToString();
        }
        else
        {
            
            Debug.LogWarning("Timer llegó a cero o menos. Valor actual: " + timeLeft);
            isRunning = false;
            keyManager.LoseGame();
        }
    }


    public void StartTimer()
    {
        
        if (initialTime <= 0)
        {
            initialTime = Totaltime;
            Debug.LogWarning("Ojo: Initial Time estaba en 0 en el Inspector. Usando 10 por defecto.");
        }

        timeLeft = initialTime;
        isRunning = true;

        if (timerText != null)
            timerText.text = initialTime.ToString("f0");

        Debug.Log("Timer iniciado con: " + timeLeft);
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}