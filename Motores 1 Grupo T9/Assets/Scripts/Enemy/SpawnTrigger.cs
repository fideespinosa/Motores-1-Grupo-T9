using UnityEngine;

[System.Serializable]
public class SpawnTrigger
{
    public resourcesManager.ResourceType resourceType;
    public int amountRequired;
    public bool alreadyTriggered;

    [Header("Enemigos ya puestos en la escena (desactivados)")]
    public GameObject[] enemiesToActivate;

    [Header("Punto donde entra al NavMesh (dentro de la cueva)")]
    public Transform navMeshEntryPoint;
}