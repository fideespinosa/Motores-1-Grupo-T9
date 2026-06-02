using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalZone : MonoBehaviour
{
    [SerializeField] SpecialResourceEvent specialResource;
    void Start()
    {
        specialResource = specialResource.GetComponent<SpecialResourceEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (specialResource.GetStatus())
            {
                SceneManager.LoadScene("FinalScene");
            }
        }
    }


}
