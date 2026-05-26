using UnityEngine;

public class resourceChecker : MonoBehaviour
{
    public narrativeResourcesManager.ItemType type;

    void Update()
    {
        bool recogido = false;

        switch (type)
        {
            case narrativeResourcesManager.ItemType.HDD:
                recogido = ControlPanelManager.Instance.hasHDD;
                break;
            case narrativeResourcesManager.ItemType.Smartphone:
                recogido = ControlPanelManager.Instance.hasSmartphone;
                break;
            case narrativeResourcesManager.ItemType.PC:
                recogido = ControlPanelManager.Instance.hasPC;
                break;
            case narrativeResourcesManager.ItemType.Tarjeta:
                recogido = ControlPanelManager.Instance.hasCard;
                break;
        }

        if (recogido)
            gameObject.SetActive(false);
    }
}
