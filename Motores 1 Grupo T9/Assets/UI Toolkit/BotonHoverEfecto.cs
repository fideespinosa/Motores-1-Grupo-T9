using GLTFast.Schema;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotonHoverEfecto : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Normal");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.Play("Highlighted");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Play("Normal");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        animator.Play("Pressed");
    }
}