using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
public class MainMenu : MonoBehaviour
{
    [Header("UI World Space (Mesa)")]
    [SerializeField] private GameObject mainMenuWorldGroup; // Padre de los UIDocuments de la mesa
    [SerializeField] private UIDocument playDocument;       // UIDocument sobre la Radio
    [SerializeField] private UIDocument optionsDocument;    // UIDocument sobre el Teléfono
    [SerializeField] private UIDocument quitDocument;       // UIDocument sobre la Mochila
    [SerializeField] private Transform nameDocument;       // UIDocument del nombre del juego

    [Header("UI Screen Space (Opciones 2D)")]
    [SerializeField] private UIDocument optionsMenu2D;

    [Header("Camara")]
    [SerializeField] private Transform camTransform;

    [Header("Panel para el fade in")]
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private float fadeDuration = 3f;

    private Button btnPlay;
    private Button btnOptions;
    private Button btnQuit;
    private Button btnBackFromOptions;

    private void OnEnable()
    {
        // 1. Vincular botones del Menú 3D (World Space)
        if (playDocument != null)
            btnPlay = playDocument.rootVisualElement.Q<Button>("PlayButton");

        if (optionsDocument != null)
            btnOptions = optionsDocument.rootVisualElement.Q<Button>("OptionButton");

        if (quitDocument != null)
            btnQuit = quitDocument.rootVisualElement.Q<Button>("QuitButton");

        // 2. Vincular botón Volver del menú de Opciones 2D
        if (optionsMenu2D != null)
            btnBackFromOptions = optionsMenu2D.rootVisualElement.Q<Button>("BackButton");

        
        btnPlay.style.display = DisplayStyle.Flex;
        btnOptions.style.display = DisplayStyle.Flex;
        btnQuit.style.display = DisplayStyle.Flex;
        btnBackFromOptions.style.display = DisplayStyle.None;

        // Suscribir eventos
        btnPlay?.RegisterCallback<ClickEvent>(OnPlayClicked);
        btnOptions?.RegisterCallback<ClickEvent>(OnOptionsClicked);
        btnQuit?.RegisterCallback<ClickEvent>(OnQuitClicked);
        btnBackFromOptions?.RegisterCallback<ClickEvent>(OnBackFromOptionsClicked);

        // Estado Inicial: Opciones ocultas
        SetOptionsMenuVisible(false);

        camTransform.position = new Vector3(268.36f, 4.81f, 820.34f);
        camTransform.rotation = Quaternion.Euler(9.681f, -68.312f, 3.826f);

        nameDocument.position = new Vector3(261.24f, 6.02f, 819.06f);
        //btnPlay.Focus();
    }


    private void OnPlayClicked(ClickEvent evt)
    {
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.FadeOutAndDestroy(15f);
        }

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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
    private void OnOptionsClicked(ClickEvent evt)
    {
        btnPlay.style.display = DisplayStyle.None;
        btnOptions.style.display = DisplayStyle.None;
        btnQuit.style.display = DisplayStyle.None;
        // Ocultar botones 3D de la mesa y mostrar panel de opciones 2D
        //mainMenuWorldGroup.SetActive(false);
        SetOptionsMenuVisible(true);

        camTransform.position = new Vector3(265.284f, 5.448f, 821.479f);
        camTransform.rotation = Quaternion.Euler(9.681f, -68.312f, 3.826f);

        nameDocument.position = new Vector3(261.24f, 4.11f, 823.01f);
    }

    private void OnBackFromOptionsClicked(ClickEvent evt)
    {
        // Ocultar opciones 2D y restaurar botones 3D de la mesa
        SetOptionsMenuVisible(false);
        //mainMenuWorldGroup.SetActive(true);

        btnPlay.style.display = DisplayStyle.Flex;
        btnOptions.style.display = DisplayStyle.Flex;
        btnQuit.style.display = DisplayStyle.Flex;

        camTransform.position = new Vector3(268.36f, 4.81f, 820.34f);
        camTransform.rotation = Quaternion.Euler(9.681f, -68.312f, 3.826f);

        nameDocument.position = new Vector3(261.24f, 6.02f, 819.06f);
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        Application.Quit();
    }

    private void SetOptionsMenuVisible(bool visible)
    {
        if (optionsMenu2D == null) return;

        VisualElement root = optionsMenu2D.rootVisualElement;
        root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        btnBackFromOptions.style.display = DisplayStyle.Flex;
    }
}
