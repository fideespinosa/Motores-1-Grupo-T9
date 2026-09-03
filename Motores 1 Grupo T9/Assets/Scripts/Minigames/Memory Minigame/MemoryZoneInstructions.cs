using UnityEngine;

public class MemoryZoneInstructions : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    
    public void ExecuteAction()
    {
        panel.SetActive(true);
    }

}
