using UnityEngine;

public class EyesEventsTrigger : MonoBehaviour
{

    [SerializeField] GameObject Blur;


    public void BlurEvent()
    {

        Blur.SetActive(true);
    }
}
