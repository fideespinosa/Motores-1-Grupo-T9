using UnityEngine;
using UnityEngine.UIElements;

public class ShipMonitorUI : MonoBehaviour
{
    // Nombres de los Label en el UXML (el campo "name" de cada uno)
    [SerializeField] string resources1Name = "Resources1Lbl";
    [SerializeField] string qResources1Name = "QResource1Lbl";
    [SerializeField] string resources2Name = "Resources2Lbl";
    [SerializeField] string qResources2Name = "QResource2Lbl";
    [SerializeField] string resources3Name = "Resources3Lbl";
    [SerializeField] string qResources3Name = "QResource3Lbl";

    Label r1, q1, r2, q2, r3, q3;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        r1 = root.Q<Label>(resources1Name);
        q1 = root.Q<Label>(qResources1Name);
        r2 = root.Q<Label>(resources2Name);
        q2 = root.Q<Label>(qResources2Name);
        r3 = root.Q<Label>(resources3Name);
        q3 = root.Q<Label>(qResources3Name);
    }

    static void Set(Label l, string s) { if (l != null) l.text = s; }

    public void ShowCounts(int metal, int combustible, int insumos)
    {
        Debug.Log($"[{name}] panel={q1?.panel?.visualTree != null} antes={q1?.text} -> {metal}", this);
        Debug.Log($"ShowCounts q1={(q1 != null)} q2={(q2 != null)} q3={(q3 != null)}");
        Set(q1, metal.ToString());
        Set(q2, combustible.ToString());
        Set(q3, insumos.ToString());
    }

    public void ShowFinal(bool collected)
    {
        Set(r1, " / 1 Anomalia");
        Set(q1, collected ? "1" : "0");
        Set(r2, ""); Set(q2, "");
        Set(r3, ""); Set(q3, "");
    }
}