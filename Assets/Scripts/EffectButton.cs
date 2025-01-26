using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    public Effect.Base effect;
    public Action action;
    public EffectListUI ui;
    public Button button;
    public TMPro.TextMeshProUGUI textMesh;

    void Start()
    {
        button.onClick.AddListener(Activate);
    }

    public void Activate()
    {
        ui.Activate(this);
    }
}
