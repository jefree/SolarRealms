using Effect;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectButton : MonoBehaviour, IPointerDownHandler
{
    public Base effect;
    public Action action;
    public EffectListUI ui;

    void Start()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.Activate(this);
    }
}
