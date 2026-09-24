using UnityEngine;
using UnityEngine.UIElements;

public class CamDroneUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement glitchOverlay;
    private VisualElement camDroneUI;
    private Label resourcesLbl;
    private Label qResourcesLbl;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        glitchOverlay = root.Q<VisualElement>("GlitchOverlay");
        camDroneUI = root.Q<VisualElement>("DronCam");
        glitchOverlay.style.display = DisplayStyle.None;
        camDroneUI.style.display = DisplayStyle.Flex;

        resourcesLbl = root.Q<Label>("ResourcesLbl");
        qResourcesLbl = root.Q<Label>("QResourceLbl");

        resourcesLbl.style.display = DisplayStyle.None;
        qResourcesLbl.style.display = DisplayStyle.None;
    }
    public void FindResource()
    {
        resourcesLbl.style.display = DisplayStyle.Flex;
        qResourcesLbl.style.display = DisplayStyle.Flex;

        resourcesLbl.text = "Recurso :";
        qResourcesLbl.text = "0";
    }
}
