using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterceptorScript : MonoBehaviour
{
    [SerializeField] private float minSeconds = 1f; //180f son 3 minutos
    [SerializeField] private float maxSeconds = 4f; //420f son 7 minutos
    [SerializeField] private float timeWhileAttacking = 10f;

    [SerializeField] private PlayerSwitcher playerSwitcher;
    [SerializeField] private GameObject alarmLight;
    [SerializeField] private AnimationManagerMemory animatorScript;
    [SerializeField] private AlarmLightScript alarmLightScript;

    [SerializeField] private GameObject zone;

    private Coroutine attackRoutine;
    private Coroutine attackTimerRoutine;

    private void OnEnable()
    {
        TryStartAppearCycle();
    }

    private void OnDisable()
    {
        StopAllLocalCoroutines();
    }

    private void TryStartAppearCycle()
    {
        if (GameStatusScript.Instance != null && GameStatusScript.Instance.minigameRunning)
            return;

        attackRoutine = StartCoroutine(AppearTimer());
    }

    private void StopAllLocalCoroutines()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (attackTimerRoutine != null)
        {
            StopCoroutine(attackTimerRoutine);
            attackTimerRoutine = null;
        }
    }

    private IEnumerator AppearTimer()
    {
        float timer = Random.Range(minSeconds, maxSeconds);

        while (timer > 0f)
        {
            if (GameStatusScript.Instance != null && GameStatusScript.Instance.minigameRunning)
            {
                attackRoutine = null;
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        AttackPerceptor();
    }

    public void AttackPerceptor()
    {
        if (GameStatusScript.Instance != null)
            GameStatusScript.Instance.StartMinigame();

        alarmLight.SetActive(true);

        playerSwitcher.SetControl(false);
        playerSwitcher.BlockSwitching();

        ActivateZone();
        Debug.Log("aparece");



        animatorScript.StartAnimation();

        attackTimerRoutine = StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(timeWhileAttacking);

        Debug.Log("perdiste");

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Game Over - Dron");
    }

    public void Win()
    {
        Debug.Log("bicho se fue");

        if (GameStatusScript.Instance != null)
        Debug.Log("minijuego apagado");
            GameStatusScript.Instance.EndMinigame();

        animatorScript.EndAnimation();

        playerSwitcher.AllowSwitching();

        alarmLight.SetActive(false);

        StopAllLocalCoroutines();

        TryStartAppearCycle();
    }

    private void ActivateZone()
    {
        if (zone == null)
            return;

        zone.SetActive(true);
    }
}