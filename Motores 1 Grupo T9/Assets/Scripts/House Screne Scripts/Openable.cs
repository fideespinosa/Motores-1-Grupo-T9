using System.Collections;
using UnityEngine;

public class Openable : MonoBehaviour, IInteractable
{
    private enum OpenAnimationType
    {
        Rotate,
        Slide
    }

    [Header("Story Flag Gate")]
    [Tooltip("Flag que debe estar activo antes de que esta puerta reaccione con normalidad. Mientras no lo esté, el click solo reproduce el sonido de 'cerrada' sin mostrar texto ni chequear llave. Dejar vacío si no aplica.")]
    [SerializeField] private string requiredFlag;

    [Header("Required Key")]
    [Tooltip("Debe coincidir con el Item Id del CollectibleItem de la llave correspondiente. Dejar vacío si esto se abre directo, sin necesitar llave.")]
    [SerializeField] private string requiredItemId;

    [Header("Locked Message")]
    [Tooltip("Solo se usa si Required Item Id no está vacío.")]
    [TextArea]
    [SerializeField] private string lockedMessage;

    [Header("Open Animation")]
    [Tooltip("Transform que rota/se desliza al abrir. Dejar vacío para usar este mismo objeto.")]
    [SerializeField] private Transform pivot;
    [Tooltip("Rotate: para puertas, gira sobre un eje (ej. bisagra). Slide: para cajones, se desliza en línea recta.")]
    [SerializeField] private OpenAnimationType animationType = OpenAnimationType.Rotate;
    [Tooltip("Grados a rotar en cada eje al abrir (relativo a la rotación cerrada). Ej: (0, 0, 90) para una puerta.")]
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 0f, 90f);
    [Tooltip("Distancia a desplazar en espacio local al abrir (relativo a la posición cerrada). Ej: (0, 0, 0.4) para un cajón.")]
    [SerializeField] private Vector3 slideOffset = new Vector3(0f, 0f, 0.4f);
    [SerializeField] private float openDuration = 0.6f;

    [Header("Outline")]
    [SerializeField] private Outline outline;

    private bool isUnlocked = false;
    private bool isOpen = false;
    private Coroutine animationCoroutine;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool RequiresKey => !string.IsNullOrEmpty(requiredItemId);
    private bool RequiresFlag => !string.IsNullOrEmpty(requiredFlag);
    private bool FlagSatisfied => !RequiresFlag || (StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(requiredFlag));

    private void Awake()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }

        Transform target = pivot != null ? pivot : transform;
        closedRotation = target.localRotation;
        openRotation = closedRotation * Quaternion.Euler(rotationOffset);
        closedPosition = target.localPosition;
        openPosition = closedPosition + slideOffset;
    }

    public void Action()
    {
        if (isUnlocked)
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Debug.Log("Sonido de apertura");
                Open();
            }
            return;
        }

        if (!FlagSatisfied)
        {
            Debug.Log("Sonido de puerta cerrada");
            return;
        }

        if (!RequiresKey)
        {
            isUnlocked = true;
            Debug.Log("Sonido de apertura");
            Open();
            return;
        }

        bool hasKey = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItemId);

        if (hasKey)
        {
            isUnlocked = true;
            Debug.Log("Sonido de llave utilizada");
            Debug.Log("Sonido de apertura");
            Open();
        }
        else
        {
            Debug.Log("Sonido de puerta cerrada");

            if (TextPanelManager.Instance != null)
            {
                TextPanelManager.Instance.ShowText(lockedMessage);
            }
        }
    }

    private void Open()
    {
        isOpen = true;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateTo(openRotation, openPosition));
    }

    private void Close()
    {
        isOpen = false;

        Debug.Log("Sonido de puerta cerrandose");

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateTo(closedRotation, closedPosition));
    }

    private IEnumerator AnimateTo(Quaternion targetRotation, Vector3 targetPosition)
    {
        Transform target = pivot != null ? pivot : transform;
        float elapsed = 0f;

        if (animationType == OpenAnimationType.Rotate)
        {
            Quaternion startRotation = target.localRotation;

            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                target.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / openDuration);
                yield return null;
            }

            target.localRotation = targetRotation;
        }
        else
        {
            Vector3 startPosition = target.localPosition;

            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                target.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / openDuration);
                yield return null;
            }

            target.localPosition = targetPosition;
        }

        animationCoroutine = null;
    }

    public void OnHoverEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}