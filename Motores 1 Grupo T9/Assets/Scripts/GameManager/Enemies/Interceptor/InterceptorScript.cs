using System.Collections;
using UnityEngine;

public class InterceptorScript : MonoBehaviour
{
    [SerializeField] private float minSeconds = 1f; //180f son 3 minutos
    [SerializeField] private float maxSeconds = 4f; //420f son 7 minutos

    [SerializeField] private GameObject alarmLight;
    [SerializeField] private AlarmLightScript alarmLightScript;

    private Coroutine attackRoutine;

    private void OnEnable()
    {
        attackRoutine = StartCoroutine(AttackTimer());
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
    }

    private IEnumerator AttackTimer()
    {
        float waitTime = Random.Range(minSeconds, maxSeconds);

        yield return new WaitForSeconds(waitTime);

        AttackPerceptor();
    }

    public void AttackPerceptor()
    {
        alarmLight.SetActive(true);
        Debug.Log("aparece");
    }
}