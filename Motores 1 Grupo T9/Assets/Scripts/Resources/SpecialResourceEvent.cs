using UnityEngine;

public class SpecialResourceEvent : MonoBehaviour
{
    [SerializeField] GameObject enemy;
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
           gameObject.SetActive(false);
            enemy.SetActive(true);

        }
    }

}
