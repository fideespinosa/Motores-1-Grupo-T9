using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IntroTerminalComputer : MonoBehaviour
{
    private UIDocument document;
    private VisualElement root;
    private VisualElement introVE;
    private VisualElement textContainerVE;
    private Label nameLbl;
    private Label detailLbl;
    private Image logoImg;

    private string nextSceneName = "MainMenu"; //Corregir con el nombre correcto!
    private bool canContinue = false;
    private AsyncOperation sceneAsyncOp;
    private IDisposable inputEventListener;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        introVE = root.Q<VisualElement>("IntroductionVE");

        textContainerVE = root.Q<VisualElement>("TextContainerVE");
        logoImg = root.Q<Image>("LogoImg");
        nameLbl = root.Q<Label>("NameLbl");
        detailLbl = root.Q<Label>("DetailLbl");

        StartCoroutine(PrecargarEscenaRutinaria());
        StartCoroutine(ActivarInputSeguro());
    }
    private IEnumerator PrecargarEscenaRutinaria()
    {
        yield return new WaitForSeconds(0.5f); // Breve espera para estabilidad del motor

        // Comenzar la carga asíncrona de la escena objetivo
        sceneAsyncOp = SceneManager.LoadSceneAsync(nextSceneName);

        // Evitamos que la escena se active automáticamente al llegar al 100% de carga
        if (sceneAsyncOp != null)
        {
            sceneAsyncOp.allowSceneActivation = false;
        }
    }

    private IEnumerator ActivarInputSeguro()
    {
        // Esperamos 0.2 segundos. Esto evita que si venían moviendo el mouse o haciendo clic 
        // durante el video, se saltee la pantalla de "Presione una tecla" instantáneamente.
        yield return new WaitForSeconds(0.5f);

        canContinue = true;

        // Escuchar CUALQUIER entrada del Input System (Teclado, Mouse, Gamepad)
        inputEventListener = InputSystem.onAnyButtonPress.Call(_ => OnAnyKeyPressed());
    }

    private void OnAnyKeyPressed()
    {
        if (!canContinue) return;

        canContinue = false;

        // Destruir listener inmediatamente para evitar doble ejecución
        inputEventListener?.Dispose();

        // Permitir el cambio a la escena que ya estaba precargada
        if (sceneAsyncOp != null)
        {
            sceneAsyncOp.allowSceneActivation = true;
        }
        else
        {
            // Failsafe por si la carga asíncrona falló o no inició: la cargamos directo
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
