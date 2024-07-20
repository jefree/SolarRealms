using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Effect;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    public IEffect effect;
    public EffectList effectList;

    void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        effectList.Activate(this);
    }
}
