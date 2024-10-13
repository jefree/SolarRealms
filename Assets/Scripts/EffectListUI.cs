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
        currentCard = card;
        currentCard.Actions().ForEach(action => AddEffects(action));

        gameObject.SetActive(true);
    }

    void AddEffects(Action action)
    {
        var prefix = action.actionName == "scrap" ? "DESHUESAR: " : "";

        foreach (var effect in action.manualEffects)
        {
            var panelTransform = transform.Find("Panel");
            var effectGO = Instantiate(itemPrefab, panelTransform);

            effectGO.GetComponent<EffectButton>().effect = effect;
            effectGO.GetComponent<EffectButton>().action = action;
            effectGO.GetComponent<EffectButton>().ui = this;
            effectGO.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = $"{prefix}{effect.Text()}";
        }
    }

    public void Activate(EffectButton button)
    {
        Close();

        //game.ResolveAction(button.action, button.effect);
        game.localPlayer.CmdResolveAction(currentCard, button.action.actionName, button.effect.ID(), button.effect.isManual);
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
