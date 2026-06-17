using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("animar puerta y panel para arriba");
    }

    private void OnDisable()
    {
        Debug.Log("animar puerta y panel para abajo");
    }
}
