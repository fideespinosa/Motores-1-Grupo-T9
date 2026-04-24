using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyQueueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentKeyText;
    [SerializeField] Timer timer;

    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float loadingTime = 5f;

    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] int targetScore = 15;

    int currentScore = 0;
    bool gameStarted = false;
    bool gameEnded = false;

    private KeyCode[] availableKeys =
    {
        KeyCode.Q,
        KeyCode.W,
        KeyCode.R,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
    };

    Queue<KeyCode> keyQueue = new Queue<KeyCode>();

    void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    void Update()
    {
        if (!gameStarted || gameEnded) return;

        HandleInput();
    }

    void InitializeQueue()
    {
        keyQueue.Clear();
        EnqueueRandomKey();
        EnqueueRandomKey();
    }

    void EnqueueRandomKey()
    {
        int index = Random.Range(0, availableKeys.Length);
        keyQueue.Enqueue(availableKeys[index]);
    }

    void HandleInput()
    {
        foreach (KeyCode key in availableKeys)
        {
            if (Input.GetKeyDown(key))
            {
                CheckKey(key);
            }
        }
    }

    void CheckKey(KeyCode pressedKey)
    {
        if (keyQueue.Count == 0) return;

        KeyCode expectedKey = keyQueue.Peek();

        if (pressedKey == expectedKey)
        {
            currentScore++;
            Debug.Log("bien");

            if (currentScore >= targetScore)
            {
                WinGame();
                return;
            }

        }
        else
        {
            Debug.Log("mal");
            currentScore = 0;
        }

        UpdateCounter();
        AdvanceKey();
    }

    void UpdateCounter()
    {
        counterText.text = currentScore.ToString();
    }

    public void AdvanceKey()
    {
        if (keyQueue.Count == 0) return;

        keyQueue.Dequeue();
        EnqueueRandomKey();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (keyQueue.Count > 0)
        {
            currentKeyText.text = keyQueue.Peek().ToString();
        }
    }

    public void ResetQueue()
    {
        InitializeQueue();
        UpdateUI();
    }

    System.Collections.IEnumerator LoadingRoutine()
    {
        loadingText.gameObject.SetActive(true);
        currentKeyText.gameObject.SetActive(false);
        timer.enabled = false;
        timer.gameObject.SetActive(false);

        yield return new WaitForSeconds(loadingTime);

        loadingText.gameObject.SetActive(false);
        currentKeyText.gameObject.SetActive(true);
        timer.gameObject.SetActive(true);
        timer.enabled = true;

        currentScore = 0;
        UpdateCounter();

        gameStarted = true;
        InitializeQueue();
        UpdateUI();
    }

    public void WinGame()
    {
        Debug.Log("ganaste");
        gameEnded = true;
        timer.StopTimer();
        GameResult(true);
    }
    public void LoseGame()
    {
        Debug.Log("perdiste");
        gameEnded = true;
        GameResult(false);
    }

    public bool GameResult(bool result)
    {
        return result;
    }
}