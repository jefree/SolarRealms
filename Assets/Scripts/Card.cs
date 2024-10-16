using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public enum Faction : byte
{
    UNALIGNED,
    TRADE_FEDERATION,
    THE_BLOBS,
    STAR_EMPIRE,
    MACHINE_CULT
}

public enum CardType
{
    SHIP,
    BASE
}

public class SyncListCard : SyncList<Card> { }

[Serializable]
public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SyncVar] public string cardName;
    [SyncVar] public int cost;
    [SyncVar(hook = nameof(OnLocationChanged))] public Location location;
    [SyncVar] public Faction faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    [SyncVar] public Player player;
    [SyncVar] public Game game;
    [SyncVar] public CardType type; // Ship or Base

    public Action mainAction;
    public Action allyAction;
    public Action doubleAllyAction;
    public Action scrapAction;
    public int defense;
    public bool outpost;
    public GameObject combatUpPrefab;
    EffectUp effectUp;
    [HideInInspector] public Action currentAction;
    public SpriteRenderer image;
    public SpriteRenderer border;

    void Start()
    {
        effectUp = Instantiate(combatUpPrefab, transform).GetComponent<EffectUp>();
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{cardName}");
    }

    void Update()
    {

        if (location != Location.PLAY_AREA)
        {
            border.color = Color.clear;
            return;
        }

        if (HasPendingActions(manual: true, includeScrap: false))
        {
            border.color = Color.green;
        }
        else if (scrapAction != null)
        {
            border.color = Color.red;
        }
    }

    [Client]
    public override void OnStartClient()
    {
        CardFactory.Build(this, game);
    }

    void OnLocationChanged(Location old, Location current)
    {
        if (old == Location.HAND && current == Location.DISCARD_PILE)
        {
            player.discardPile.OnCardAdded(this);
        }
        else if (old == Location.DISCARD_PILE && current == Location.DECK)
        {
            player.deck.OnCardInserted(new CardInfo(this));
        }
    }

    public void Activate()
    {
        ActivateNextAction();
    }

    void ActivateNextAction()
    {
        currentAction = NextAction();

        if (currentAction == null)
        {
            game.CardResolved(this);
            return;
        }

        currentAction.Activate();
    }

    public void ActionResolved(Action action)
    {
        if (action.actionName == "scrap")
        {
            game.CardResolved(this);
            game.ScrapCard(this);
            return;
        }

        ActivateNextAction();
    }

    public Action NextAction()
    {
        return ActualActions(mainAction, allyAction, doubleAllyAction).Find(action => action.HasPendingEffects());
    }

    public void Reset()
    {
        ActualActions(mainAction, allyAction, doubleAllyAction, scrapAction).ForEach(action => action.Reset());
    }

    public List<Action> Actions()
    {
        return ActualActions(mainAction, allyAction, doubleAllyAction, scrapAction);
    }

    List<Action> ActualActions(params Action[] actions)
    {
        var validActions = actions.ToList();
        validActions.RemoveAll(action => action == null);

        if (validActions.Count == 0)
        {
            throw new InvalidOperationException($"card with no actions '{cardName}'");
        }

        return validActions;
    }

    public bool HasPendingActions(bool manual = false, bool includeScrap = true)
    {
        var actions = new List<Action> { mainAction, allyAction, doubleAllyAction };

        if (includeScrap)
            actions.Add(scrapAction);

        return ActualActions(actions.ToArray()).Any(action => action.HasPendingEffects(manual: manual));
    }

    [ClientRpc]
    public void RpcShowEffect(EffectColor color, float value)
    {
        effectUp.Enqueue(color, value, type);
    }

    public void Hide()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Cards/back");
    }

    public void Show()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{cardName}");
    }

    public Action FindAction(string name)
    {
        return name switch
        {
            "main" => mainAction,
            "scrap" => scrapAction,
            "ally" => allyAction,
            "doubleAlly" => doubleAllyAction,
            _ => throw new ArgumentException($"Invalid action name {name} in card {cardName}")
        };
    }

    [Client]
    public void OnPointerDown(PointerEventData data)
    {
        // invalidate click on cards outside card list
        if (game.discardPileList.gameObject.activeSelf && location != Location.DISCARD_PILE)
            return;

        if (location == Location.DISCARD_PILE && !game.discardPileList.gameObject.activeSelf)
        {
            game.ShowDiscardCards(player);
            return;
        }

        // click other player's hand card
        if (
            game.activePlayer != game.localPlayer ||
            (location == Location.HAND && player != game.localPlayer)
        )
        {
            return;
        }

        if (
           game.state == GameState.CHOOSE_CARD &&
           game.localPlayer.IsOurTurn()
        )
        {
            game.localPlayer.CmdChooseCard(this);
            return;
        }


        // probably attacking a base
        if (
            game.state == GameState.DO_BASIC &&
            location == Location.PLAY_AREA &&
            game.localPlayer != player &&
            type == CardType.BASE
        )
        {
            game.localPlayer.CmdAttackBase(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.PLAY_AREA
        )
        {
            game.ShowEffectList(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.HAND
        )
        {

            player.CmdPlayCard(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.TRADE_ROW
        )
        {
            game.localPlayer.CmdBuyCard(this);
            return;
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (location == Location.DISCARD_PILE) { return; }

        transform.localScale = new Vector2(1.2f, 1.2f);
        image.sortingOrder = 2;
        border.sortingOrder = 1;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (location == Location.DISCARD_PILE) { return; }

        transform.localScale = Vector2.one;
        image.sortingOrder = 0;
        border.sortingOrder = -1;
    }
}