using UnityEngine;

public class SecondScene : MonoBehaviour
{
    [SerializeField] private float delayTime = 5f;
    [SerializeField] private GameObject panelToEnable;

    void Start()
    {
        Invoke(nameof(EnablePanel), delayTime);
    }

    void EnablePanel()
    {
        panelToEnable.SetActive(true);
    }
}