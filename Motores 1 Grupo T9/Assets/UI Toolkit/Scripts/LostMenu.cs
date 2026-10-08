using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LostMenu : MonoBehaviour
{
    [Header("UI World Space (Mesa)")]
    [SerializeField] private UIDocument playDocument;
    [SerializeField] private UIDocument menuDocument;

    [Header("Panel para el fade in")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private float fadeDuration = 3f;

    [SerializeField] private string sceneName = "MainMenu OK";

    private Button btnPlay;
    private Button btnMenu;

    private UIDocument uIDocument;
    private VisualElement gameOverVE;

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playDocument != null)
            btnPlay = playDocument.rootVisualElement.Q<Button>("PlayButton");
        if (menuDocument != null)
            btnMenu = menuDocument.rootVisualElement.Q<Button>("MenuButton");

        uIDocument = GetComponent<UIDocument>();
        var root = uIDocument.rootVisualElement;
        gameOverVE = root.Q<VisualElement>("GameOverVE");

        gameOverVE.pickingMode = PickingMode.Ignore;

        // Suscribir eventos
        btnPlay?.RegisterCallback<ClickEvent>(OnPlayClicked);
        // Suscribir eventos
        btnMenu?.RegisterCallback<ClickEvent>(OnMenuClicked);
    }
    private void OnDisable()
    {
        // Suscribir eventos
        btnPlay?.UnregisterCallback<ClickEvent>(OnPlayClicked);
        // Suscribir eventos
        btnMenu?.UnregisterCallback<ClickEvent>(OnMenuClicked);
    }
    public void OnPlayClicked(ClickEvent evt)
    {
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.FadeOutAndDestroy(15f);
        }
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Cargando juego...");
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float time = 0f;
        panel.alpha = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        panel.alpha = 1f;
        SceneManager.LoadScene("Level1");
    }
    public void OnMenuClicked(ClickEvent evt)
    {
        SceneManager.LoadScene(sceneName);
    }
}
