using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections;

public class Computer : MonoBehaviour
{
    public class DatosFormulario
    {
        public string nombre;
        public string email;
        public string edad;
        public string vinculosAfectivos;
    }

    public DatosFormulario datosUsuarioActual = new DatosFormulario();

    private UIDocument document;
    private VisualElement rootElement;
    private VisualElement screenWeb;
    private VisualElement screenNotAvailable;
    private TextField nameTF;
    private TextField emailTF;
    private TextField ageTF;
    private DropdownField webLinkDF;
    private Button sendButton;
    private Label sendFormLabel;
    private Label errorLabel;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        rootElement = document.rootVisualElement;
        screenWeb = rootElement.Q<VisualElement>("ScreenWeb");
        screenNotAvailable = rootElement.Q<VisualElement>("ScreenNotAvailable");
        nameTF = rootElement.Q<TextField>("NombreTextField");
        emailTF = rootElement.Q<TextField>("EmailTextField");
        ageTF = rootElement.Q<TextField>("EdadTextField");
        webLinkDF = rootElement.Q<DropdownField>("VinculosDropdownField");
        sendButton = rootElement.Q<Button>("EnviarButton");
        sendFormLabel = rootElement.Q<Label>("SendFormLabel");
        errorLabel = rootElement.Q<Label>("ErrorLabel");
        

        screenWeb.style.display = DisplayStyle.Flex;
        screenNotAvailable.style.display = DisplayStyle.None;

        if(sendFormLabel != null) sendFormLabel.style.display= DisplayStyle.None;
        if (nameTF != null) nameTF.style.display = DisplayStyle.Flex;
        if (emailTF != null) emailTF.style.display = DisplayStyle.Flex;
        if (ageTF != null) ageTF.style.display = DisplayStyle.Flex;
        if (webLinkDF != null) webLinkDF.style.display = DisplayStyle.Flex;
        if (sendButton != null) sendButton.style.display = DisplayStyle.Flex;
        if (errorLabel != null) errorLabel.style.display = DisplayStyle.None; 
        

        webLinkDF.choices = new List<string>()
        {
            "Seleccione una opción...",
            "No, no tengo vínculos afectivos.",
            "Sí, dejaría seres queridos atrás."
        };

        // Opción marcada por defecto
        webLinkDF.value = webLinkDF.choices[0];

        if (sendButton != null)
        {
            sendButton.clicked += OnFormSubmit;
        }

        // Cada vez que el usuario escriba una letra o cambie el dropdown, borramos el error de la pantalla
        nameTF.RegisterCallback<ChangeEvent<string>>(evt => HideInstantError());
        emailTF.RegisterCallback<ChangeEvent<string>>(evt => HideInstantError());
        ageTF.RegisterCallback<ChangeEvent<string>>(evt => HideInstantError());
        webLinkDF.RegisterCallback<ChangeEvent<string>>(evt => HideInstantError());
    }
    private void OnDisable()
    {
        if (sendButton != null)
        {
            sendButton.clicked -= OnFormSubmit;
        }
    }
    void OnFormSubmit()
    {
        if (!ValidateForm())
        {
            return;
        }

        // Si todos los campos están llenos, ocultamos cualquier error previo
        if (errorLabel != null) errorLabel.style.display = DisplayStyle.None;

        // Proceso de guardado normal
        datosUsuarioActual.nombre = nameTF.value;
        datosUsuarioActual.email = emailTF.value;
        datosUsuarioActual.edad = ageTF.value;
        datosUsuarioActual.vinculosAfectivos = webLinkDF.value;

        Debug.Log("[Sistema] Formulario aprobado y guardado con éxito.");
        //GuardarDatosEnDisco();

        sendFormLabel.style.display = DisplayStyle.Flex;
        nameTF.style.display = DisplayStyle.None;
        emailTF.style.display = DisplayStyle.None;
        ageTF.style.display = DisplayStyle.None;
        webLinkDF.style.display = DisplayStyle.None;
        sendButton.style.display = DisplayStyle.None;

        StartCoroutine(SendForm());
    }
    private bool ValidateForm()
    {
        // 1. Validar Nombre (Verifica que no esté vacío o solo espacios)
        if (string.IsNullOrWhiteSpace(nameTF.value))
        {
            ShowError("Falta completar el campo: Nombre Completo.");
            return false;
        }

        // 2. Validar Email
        if (string.IsNullOrWhiteSpace(emailTF.value))
        {
            ShowError("Falta completar el campo: Dirección de e-mail.");
            return false;
        }

        // 3. Validar Edad
        if (string.IsNullOrWhiteSpace(ageTF.value))
        {
            ShowError("Falta completar el campo: Edad.");
            return false;
        }

        // 4. Validar el Dropdown (Índice 0 no es válido)
        if (webLinkDF.index == 0)
        {
            ShowError("Por favor, responda la pregunta sobre vínculos afectivos.");
            return false;
        }

        return true; // Todo está en orden
    }

    // Método auxiliar para centralizar las alertas
    private void ShowError(string mensaje)
    {
        Debug.LogWarning($"[Validación] {mensaje}");

        if (errorLabel != null)
        {
            errorLabel.text = mensaje;
            errorLabel.style.display = DisplayStyle.Flex; // Muestra el cartel en UI
        }
    }
    private void HideInstantError()
    {
        if (errorLabel != null && errorLabel.style.display == DisplayStyle.Flex)
        {
            errorLabel.style.display = DisplayStyle.None;
        }
    }
    IEnumerator SendForm()
    {
        yield return new WaitForSeconds(2f);
        sendFormLabel.style.display = DisplayStyle.None;
        nameTF.style.display = DisplayStyle.Flex;
        emailTF.style.display = DisplayStyle.Flex;
        ageTF.style.display = DisplayStyle.Flex;
        webLinkDF.style.display = DisplayStyle.Flex;
        sendButton.style.display = DisplayStyle.Flex;
    }
    private void GuardarDatosEnDisco()
    {
        // Convierte la clase a un texto en formato JSON
        string json = JsonUtility.ToJson(datosUsuarioActual, true);

        // Define la ruta del archivo (se guardará en la carpeta del juego)
        string rutaArchivo = Path.Combine(Application.persistentDataPath, "registro_recluta.json");

        // Escribe el archivo en el almacenamiento del sistema
        File.WriteAllText(rutaArchivo, json);

        Debug.Log($"[Disco] Datos guardados permanentemente en: {rutaArchivo}");
    }
}
