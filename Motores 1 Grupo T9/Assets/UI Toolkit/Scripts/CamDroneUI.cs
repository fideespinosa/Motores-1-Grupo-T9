using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
    private Label listObjetiveLbl;
    private Label objectiveLbl;
    private Label timeLbl;
    private Label signalLbl;

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
        listObjetiveLbl = root.Q<Label>("ListObjetivesLbl");
        objectiveLbl = root.Q<Label>("ObjetivesLbl");
        timeLbl = root.Q<Label>("TimeLbl");
        signalLbl = root.Q<Label>("SignalLbl");

        signal = 80f;
        signalLbl.text = $"Señal : {signal:F2} %";

        /*resources1Lbl.text = "/ 1  Piezas de Metal";
        resources2Lbl.text = "/ 1  Posible Combustible";
        resources3Lbl.text = "/ 1  Insumos Electrónicos";
        qResources1Lbl.text = "0";
        qResources2Lbl.text = "0";
        qResources3Lbl.text = "0";*/
        //resources1Lbl.style.display = DisplayStyle.None;
        //qResources1Lbl.style.display = DisplayStyle.None;
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
    public void FindResource()
    {
        //resources1Lbl.style.display = DisplayStyle.Flex;
        //qResources1Lbl.style.display = DisplayStyle.Flex;
/*
        resources1Lbl.text = "Recurso1 :";
        qResources1Lbl.text = "0";
        resources2Lbl.text = "Recurso2 :";
        qResources2Lbl.text = "0";
        resources3Lbl.text = "Recurso3 :";
        qResources3Lbl.text = "0";*/
    }
    public void ObjetivesList(string objectivo)
    {
        listObjetiveLbl.text = objectivo;
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


        //signalLbl.text = "Señal : 80 %";

        //yield return new WaitForSeconds(3f);
        //signalLbl.text = "Señal : 90 %";

        //yield return new WaitForSeconds(4f);
        //signalLbl.text = "Señal : 70 %";
    }
}
