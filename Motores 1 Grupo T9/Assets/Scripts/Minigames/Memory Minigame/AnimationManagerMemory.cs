using UnityEngine;

public class AnimationManagerMemory : MonoBehaviour
{
    [SerializeField] private GameObject CompuertaAnimator;

    private Animator animator;

    private void Start()
    {
        animator = CompuertaAnimator.GetComponent<Animator>();
    }

    public void StartAnimation()
    {

            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");

    }

    public void EndAnimation()
    {


            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");

    }
}