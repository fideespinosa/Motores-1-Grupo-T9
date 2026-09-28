using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] TransitionToCave Script;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Script.StartAnimation();
        }

    }
    public void DisableHud()
    {
        /*playerMovement.enabled = false;
        HUD.SetActive(false);
        PreviousHUD.SetActive(false);
        Debug.Log("asdasdsad");*/
    }

}
