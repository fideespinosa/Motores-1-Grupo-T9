using UnityEngine;

public class SpecialResourceEvent : MonoBehaviour
{
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
