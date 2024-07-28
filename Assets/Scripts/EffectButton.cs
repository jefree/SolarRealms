using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Effect;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    public Base effect;
    public Action action;
    public EffectListUI ui;

    void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        ui.Activate(this);
    }
}
