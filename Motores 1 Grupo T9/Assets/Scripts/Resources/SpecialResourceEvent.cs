using UnityEngine;
using TMPro;
public class SpecialResourceEvent : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] GameObject enemy;
    bool isTaken = false;
   // [SerializeField] GameObject particles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.text = "1";
            isTaken = true; 
           gameObject.SetActive(false);
           enemy.SetActive(true);
        }
    }

    public bool GetStatus()
    {
        return isTaken;
    }

}
