using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyQueueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentKeyText;
    [SerializeField] Timer timer;

    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float loadingTime = 5f;

    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] int targetScore = 5;
    protected PlayerSwitcher switcher;

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
        if (keyQueue.Count == 0 || gameEnded) return;

        KeyCode expectedKey = keyQueue.Peek();

        if (pressedKey == expectedKey)
        {
            currentScore++;
            Debug.Log("Bien! Score: " + currentScore);

            UpdateCounter();

           
            if (currentScore >= targetScore)
            {
                WinGame();
                return; 
            }

           
            AdvanceKey();
        }
        else
        {
            Debug.Log("Error! Reiniciando racha");
            currentScore = 0;
            UpdateCounter();
            
        }
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
        
        gameEnded = false;
        gameStarted = false;
        currentScore = 0;

        loadingText.gameObject.SetActive(true);
        currentKeyText.gameObject.SetActive(false);

        
        yield return new WaitForSecondsRealtime(loadingTime);

        loadingText.gameObject.SetActive(false);
        currentKeyText.gameObject.SetActive(true);

        
        InitializeQueue();
        UpdateUI();
        gameStarted = true;

        
        timer.StartTimer();
    }

    public void WinGame()
    {
        gameEnded = true;
        timer.StopTimer();

        
        Time.timeScale = 1f;

        EnemyMovement enemy = Object.FindFirstObjectByType<EnemyMovement>();
        if (enemy != null)
        {
            
            enemy.ResetEnemy();
        }

        if (switcher == null) switcher = Object.FindFirstObjectByType<PlayerSwitcher>();

        if (switcher != null)
        {
            
            switcher.enabled = true;
                
            switcher.SetControl(true);
        }

      
        transform.root.gameObject.SetActive(false);

        Debug.Log("Mundo descongelado y control devuelto al Dron.");
    }

    public void LoseGame()
    {
        Debug.Log("FALLO DE CONEXIÓN: Dron perdido.");
        gameEnded = true;

        Time.timeScale = 1f;

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

       
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Over - Dron");
    }
    public bool GameResult(bool result)
    {
        return result;
    }
}