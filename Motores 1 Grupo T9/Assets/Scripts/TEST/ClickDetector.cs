using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ClickDetector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask layerMask = ~0;

    private readonly List<RaycastResult> uiResults = new List<RaycastResult>();

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // 1. Cursor bloqueado u oculto
        if (Cursor.lockState != CursorLockMode.None)
            Debug.LogWarning($"Cursor bloqueado: {Cursor.lockState} (el click sale del centro de la pantalla)");
        if (!Cursor.visible)
            Debug.LogWarning("Cursor oculto (Cursor.visible = false)");

        // 2. UI que bloquea el click
        foreach (var doc in FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
        {
            var root = doc.rootVisualElement;
            if (root?.panel == null) continue;

            Vector2 screenPos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(root.panel, screenPos);
            var picked = root.panel.Pick(panelPos);

            if (picked != null)
            {
                Debug.LogWarning($"UI Toolkit bloqueando: {picked.name} (en {doc.name}), pickingMode: {picked.pickingMode}");
                return;
            }
        }

        // 3. Objeto 3D tocado
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            Debug.Log($"Tocaste: {hit.collider.name} | Posición: {hit.point}");
        else
            Debug.Log("No tocaste nada");
    }
}