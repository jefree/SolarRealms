using Effect;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour, IPointerDownHandler
{
    public Base effect;
    public Action action;
    public EffectListUI ui;
    public Button button;
    public TMPro.TextMeshProUGUI textMesh;

    void Start()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.Activate(this);
    }
}
