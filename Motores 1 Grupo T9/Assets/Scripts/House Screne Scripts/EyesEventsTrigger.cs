using UnityEngine;

public class EyesEventsTrigger : MonoBehaviour
{

    [SerializeField] GameObject Blur;
    [SerializeField] GameObject DronHud;


    public void BlurEvent()
    {

        Blur.SetActive(true);
        DronHud.SetActive(true);
    }
}
