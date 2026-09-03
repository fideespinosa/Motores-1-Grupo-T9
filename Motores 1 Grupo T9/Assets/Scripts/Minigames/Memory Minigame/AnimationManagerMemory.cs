using UnityEngine;

public class AnimationManagerMemory : MonoBehaviour
{
    [SerializeField] private GameObject CompuertaAnimator;
    [SerializeField] private GameObject PanelAnimator;

    private Animator animatorCompuerta;
    private Animator animatorPanel;
    private void Start()
    {
        animatorCompuerta = CompuertaAnimator.GetComponent<Animator>();
        animatorPanel = PanelAnimator.GetComponent<Animator>();
    }

    public void StartAnimation()
    {

        animatorCompuerta.ResetTrigger("Close");
        animatorCompuerta.SetTrigger("Open");

        animatorPanel.ResetTrigger("Down");
        animatorPanel.SetTrigger("Up");

    }

    public void EndAnimation()
    {
        animatorPanel.SetTrigger("Down");
        animatorPanel.ResetTrigger("Up");

        animatorCompuerta.ResetTrigger("Open");
        animatorCompuerta.SetTrigger("Close");

    }
}