using UnityEngine;

public class ActivateMateToDrink : MonoBehaviour
{

    [SerializeField] GameObject mate;
    [SerializeField] GameObject termo;
    [SerializeField] WaterStream water;
    [SerializeField] AnimationItem animationItem;
    [SerializeField] Outline outlineTermo;
    [SerializeField] Outline outlineMate;
    [SerializeField] SoundItem soundItem;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip termoClip;

    private void Start()
    {
        mate.tag = "Untagged";
        mate.layer = 0;
    }
    public void EnableDrinkMate()
    {
        mate.tag = "Item";
        mate.layer = LayerMask.NameToLayer("Item");
      //  outlineMate.enabled = true;
        soundItem.enabled = true;

    }

    public void DisableDrinkMate()
    {
        mate.tag = "Untagged";
        mate.layer = 0;
        termo.tag = "Item";
        termo.layer = LayerMask.NameToLayer("Item");
    }

    public void DisableTermoAction()
    {
        termo.tag = "Untagged";
        termo.layer = 0;

    }
    public void OpenWater()
    {
        water.Open();
        audioSource.clip = termoClip;
        audioSource.Play();
    }

    public void closeWater()
    {
        water.Close();
        audioSource.Stop();
    }

}
