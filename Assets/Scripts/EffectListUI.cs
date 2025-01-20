using UnityEngine;
using UnityEngine.UI;

public class EffectListUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public Game game;
    public UIManager ui;
    Card currentCard;

    void Update()
    {

    }

    public void Show(Card card)
    {
        ui.ShowNew(GetComponent<UIDisplay>());

        currentCard = card;
        currentCard.Actions().ForEach(action => AddEffects(action));

        gameObject.SetActive(true);
    }

    void AddEffects(Action action)
    {

        if (!action.SatisfyConditions())
        {
            return;
        }

        // improve this, maybe each kind of action can draw in the list with its own style
        var prefix = action.actionName == "scrap" ? "DESHUESAR: " : "";
        prefix = prefix == "" ? $"{action.PrefixText()} " : "";
        var color = action.actionName == "scrap" ? Color.red : Color.green;

        foreach (var effect in action.manualEffects)
        {
            AddEffect(effect, action, color, prefix);
        }
    }

    void AddEffect(Effect.Base effect, Action action, Color color, string prefix = "")
    {
        var panelTransform = transform.Find("Panel");
        var effectButton = Instantiate(itemPrefab, panelTransform).GetComponent<EffectButton>();

        effectButton.effect = effect;
        effectButton.action = action;
        effectButton.ui = this;

        effectButton.textMesh.text = $"{prefix}{effect.Text()}";

        var newColors = effectButton.button.colors;
        newColors.normalColor = color;
        effectButton.button.colors = newColors;
    }

    public void Activate(EffectButton button)
    {
        Close();
        button.effect.Dispatch(game);

        // game.localPlayer.CmdResolveAction(button.effect.ToNet());
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
