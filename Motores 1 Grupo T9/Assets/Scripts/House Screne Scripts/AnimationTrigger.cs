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

}
