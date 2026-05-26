using System.Collections;
using TMPro;
using UnityEngine;

public class narrativeResourcesManager : MonoBehaviour
{
    public enum ItemType { HDD, Smartphone, PC, Tarjeta }
    public ItemType type;
    public GameObject narrativePanel;

    public TextMeshProUGUI alertText;

    public GameObject showObjectOnPC;

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player")) return;

        // Debug.Log("Colisionó con: " + other.name);


        switch (type)
        {
            case ItemType.Tarjeta:
                ControlPanelManager.Instance.RecollectCard();
                Debug.Log("Agarras la tarjeta");
                break;
            case ItemType.HDD: 
                ControlPanelManager.Instance.RecollectHDD();
                Debug.Log("Agarras el rígido");
                break;

            case ItemType.Smartphone: 
                ControlPanelManager.Instance.RecollectSmartphone();
                Debug.Log("Agarras el teléfono");
                break;

            case ItemType.PC: 
                ControlPanelManager.Instance.ReollectPC(); 
                Debug.Log("Agarras la PC/Notebook");
                break;
        }

        StartCoroutine(ShowAlertMessage());

    }

    IEnumerator ShowAlertMessage()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        alertText.gameObject.SetActive(true);
        showObjectOnPC.gameObject.SetActive(true);

        // Debug.Log("Comienza temp de 3seg");

        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);
        alertText.gameObject.SetActive(false);
    }
}