using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Open()
    {

        animator.SetTrigger("Open");
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}