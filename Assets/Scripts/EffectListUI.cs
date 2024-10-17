using UnityEngine;

public class EffectListUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public Game game;
    Card currentCard;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Show(Card card)
    {
        currentCard = card;
        currentCard.Actions().ForEach(action => AddEffects(action));

        gameObject.SetActive(true);
    }

    void AddEffects(Action action)
    {
        // improve this, maybe each kind of action can draw in the list with its own style
        var prefix = action.actionName == "scrap" ? "DESHUESAR: " : "";
        prefix = prefix == "" && action is OrAction ? "Or " : "";
        var color = action.actionName == "scrap" ? Color.red : Color.white;

        foreach (var effect in action.manualEffects)
        {
            AddEffect(effect, action, color, prefix);
        }
    }

    void AddEffect(Effect.Base effect, Action action, Color color, string prefix = "")
    {
        var panelTransform = transform.Find("Panel");
        var effectGO = Instantiate(itemPrefab, panelTransform);

        effectGO.GetComponent<EffectButton>().effect = effect;
        effectGO.GetComponent<EffectButton>().action = action;
        effectGO.GetComponent<EffectButton>().ui = this;

        var textGUI = effectGO.GetComponent<TMPro.TextMeshProUGUI>();

        textGUI.color = color;
        textGUI.text = $"{prefix}{effect.Text()}";
    }

    public void Activate(EffectButton button)
    {
        Close();

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
