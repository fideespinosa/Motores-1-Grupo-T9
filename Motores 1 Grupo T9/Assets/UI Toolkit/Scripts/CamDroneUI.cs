using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CamDroneUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement camDroneUI;
    public Label resources1Lbl;
    public Label qResources1Lbl;
    public Label resources2Lbl;
    public Label qResources2Lbl;
    public Label resources3Lbl;
    public Label qResources3Lbl;
    private Label objectiveLbl;
    private Label timeLbl;
    private Label signalLbl;
    private Image redImg;

    [SerializeField] private DateTime date = new DateTime(2103, 3, 9);
    private float time = 0f;
    private float signal = 0f;


    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        camDroneUI = root.Q<VisualElement>("DronCam");
        camDroneUI.style.display = DisplayStyle.Flex;

        resources1Lbl = root.Q<Label>("Resources1Lbl");
        qResources1Lbl = root.Q<Label>("QResource1Lbl");
        resources2Lbl = root.Q<Label>("Resources2Lbl");
        qResources2Lbl = root.Q<Label>("QResource2Lbl");
        resources3Lbl = root.Q<Label>("Resources3Lbl");
        qResources3Lbl = root.Q<Label>("QResource3Lbl");
        objectiveLbl = root.Q<Label>("ObjetivesLbl");
        timeLbl = root.Q<Label>("TimeLbl");
        signalLbl = root.Q<Label>("SignalLbl");

        redImg = root.Q<Image>("RedImg");

        signal = 80f;
        signalLbl.text = $"Señal : {signal:F2} %";

        // Configuramos la transición por código para que el desvanecimiento sea suave
        redImg.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) });
        redImg.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });

        // Iniciamos el latido continuo
        BeatImage();

    }
    private void Start()
    {
        // Configuramos la hora inicial aquí (en horas de 0 a 23)
        int horaInicial = 14; // Por ejemplo, que empiece a las 14:00 hs
        int minutoInicial = 30; // Y 30 minutos (14:30 hs)

        // Convertimos esa hora inicial a segundos para la variable 'time'
        time = (horaInicial * 3600f) + (minutoInicial * 60f);

        
    }
    private void Update()
    {
        //FindResource();
        ActualizeTime(100f);
        Signal();
    }
    
    public void ActualizeTime(float tiempoEnSegundos)
    {
        if (tiempoEnSegundos < 0f) tiempoEnSegundos = 0f;

        // Acumulamos el tiempo que pasa
        time += tiempoEnSegundos * Time.deltaTime;

        // 86400 segundos equivalen a 24 horas (un día completo)
        while (time >= 86400f)
        {
            date = date.AddDays(1);
            time -= 86400f; // Restamos un día en segundos para continuar el conteo
        }
        // Calculamos horas, minutos y segundos restantes del día actual
        int horas = Mathf.FloorToInt(time / 3600f);
        int minutos = Mathf.FloorToInt((time % 3600f) / 60f);
        int segundos = Mathf.FloorToInt(time % 60f);

        // Formateamos la fecha (dd/MM/yyyy) y el tiempo infinito
        string fechaFormateada = date.ToString("dd/MM/yyyy");

        // Si querés mostrar solo Minutos:Segundos como antes, pero que avancen sin límite el mismo día:
        // NOTA: Si querés incluir las horas en el formato, usá "{1:00}:{2:00}:{3:00}"
        int minutosTotalesDelDia = (horas * 60) + minutos;

        timeLbl.text = $"Fecha: {fechaFormateada} | " + string.Format("{0:00}:{1:00}", horas, minutos, segundos);
    }
    public void Signal()
    {
        StartCoroutine(DelaySignal());
    }
    IEnumerator DelaySignal()
    {
        float signalNew = 70f;
        float valorInicial = signal;
        float tiempoTranscurrido = 0f;
        float tiempoTransicion = 0.2f;

        yield return new WaitForSeconds(5f);
        while (tiempoTranscurrido < tiempoTransicion)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Calculamos el porcentaje de progreso (va de 0 a 1)
            float progreso = tiempoTranscurrido / tiempoTransicion;

            // Interpolamos gradualmente entre el valor inicial y el objetivo
            signal = Mathf.Lerp(valorInicial, signalNew, progreso);

            signalLbl.text = $"Señal : {signal:F2} %";

            // Espera al próximo frame para que se note la transición visual/numérica
            yield return null;
        }
        signal = signalNew;
        signalLbl.text = $"Señal : {signal:F2} %";

    }
    public void SetHudDroneVisible(bool visible, int metal, int combustible, int insumos)
    {
        if (camDroneUI == null) return;

        //VisualElement root = optionsMenu2D.rootVisualElement;
        camDroneUI.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        //btnBackFromOptions.style.display = DisplayStyle.Flex;
        if (visible)
        {
            UpdateResources(metal, combustible, insumos);
        }
    }
    public void UpdateResources(int metal, int combustible, int insumos)
    {
        qResources1Lbl.text = metal.ToString();
        qResources2Lbl.text = combustible.ToString();
        qResources3Lbl.text = insumos.ToString();
    }
    private void BeatImage()
    {
        // Programamos una tarea repetitiva cada 1000 milisegundos (1 segundo)
        redImg.schedule.Execute(() =>
        {
            // Si la opacidad actual es cercana a 1 (visible), la bajamos a 0. Sino, a 1.
            if (redImg.style.opacity.value > 0.5f)
            {
                redImg.style.opacity = 0f;
            }
            else
            {
                redImg.style.opacity = 1f;
            }
        }).Every(1000); // Se ejecuta continuamente cada 1 segundo
    }
}
