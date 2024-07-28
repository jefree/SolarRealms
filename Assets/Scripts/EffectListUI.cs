using System.Collections;
using System.Collections.Generic;
using Effect;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectListUI : MonoBehaviour
{

    public GameObject itemPrefab;
    public Game game;
    Card currentCard;

    public void Show(Card card)
    {
        Debug.Log($"show effects for {card.cardName}");

        currentCard = card;

        AddEffects(card.mainAction);
        AddEffects(card.scrapAction);

        gameObject.SetActive(true);
    }

    void AddEffects(Action action)
    {
        foreach (var effect in action.effects)
        {
            var panelTransform = transform.Find("Panel");
            var effectGO = Instantiate(itemPrefab, panelTransform);

            effectGO.GetComponent<EffectButton>().effect = effect;
            effectGO.GetComponent<EffectButton>().action = action;
            effectGO.GetComponent<EffectButton>().ui = this;
            effectGO.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = effect.Text();
        }
    }

    public void Activate(EffectButton button)
    {
        Close();

        game.ResolveAction(button.action);
        button.action.ActivateEffect(button.effect);
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
