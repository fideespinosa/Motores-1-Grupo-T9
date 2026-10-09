using UnityEngine;

public class TestCinematic : MonoBehaviour
{
    [SerializeField] CinematicManager cinematic;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            cinematic.StartMonsterCinematic();
    }
}