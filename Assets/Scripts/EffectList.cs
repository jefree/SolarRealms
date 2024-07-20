using System.Collections;
using System.Collections.Generic;
using Effect;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectList : MonoBehaviour
{

    public GameObject itemPrefab;
    public Game game;

    public void Show(Card card)
    {
        Debug.Log($"show effects for {card.cardName}");

        foreach (var effect in card.effects)
        {
            var panelTransform = transform.Find("Panel");
            var effectGO = Instantiate(itemPrefab, panelTransform);

            effectGO.GetComponent<EffectButton>().effect = effect;
            effectGO.GetComponent<EffectButton>().effectList = this;
            effectGO.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = effect.Text();
        }

        gameObject.SetActive(true);
    }

    public void Activate(EffectButton button)
    {
        Close();
        game.ResolveEffect(button.effect);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        var panelTransform = transform.Find("Panel");

        foreach (Transform item in panelTransform)
        {
            Destroy(item.gameObject);
        }
    }
}
