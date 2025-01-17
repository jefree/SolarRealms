using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
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
    BASE,

    SHIP_BASE = SHIP | BASE
}

public class SyncListCard : SyncList<Card> { }

[Serializable]
public class Card : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SyncVar] public string cardName;
    [SyncVar] public int cost;
    [SyncVar(hook = nameof(OnLocationChanged))] public CardLocation location;
    [SyncVar] public Faction faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    [SyncVar] public Player player;
    [SyncVar] public Game game;
    [SyncVar] public CardType type; // Ship or Base

    public bool isSelected;
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

    public void Init()
    {
        mainAction = new Action(this, "main");
        allyAction = new AllyCardAction(this, "ally");
        doubleAllyAction = new DoubleAllyCardAction(this, "doubleAlly");
        scrapAction = new ScrapAction(this, "scrap");
    }

    void Start()
    {
        effectUp = Instantiate(combatUpPrefab, transform).GetComponent<EffectUp>();
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{cardName}");
    }

    void Update()
    {
        border.color = Color.clear;

        if (!game.localPlayer.IsOurTurn())
        {
            return;
        }

        if (isSelected)
        {
            border.color = Color.yellow;
            return;
        }

        if (location == CardLocation.PLAY_AREA)
        {
            if (HasPendingActions(manual: true, includeScrap: false))
            {
                border.color = Color.green;
            }
            else if (scrapAction.HasPendingEffects(manual: true))
            {
                border.color = Color.red;
            }

            return;
        }
    }

    [Client]
    public override void OnStartClient()
    {
        CardFactory.Build(this);
    }

    void OnLocationChanged(CardLocation old, CardLocation current)
    {
        if (old == CardLocation.HAND && current == CardLocation.DISCARD_PILE)
        {
            player.discardPile.OnCardAdded(this);
        }
        else if (old == CardLocation.DISCARD_PILE && current == CardLocation.DECK)
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
            game.OnCardResolved(this);
            return;
        }

        currentAction.Activate();
    }

    [Server]
    public void ActionResolved(Action action)
    {
        if (action.actionName == "scrap")
        {
            game.OnCardResolved(this);
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

    public void NetReset()
    {
        Reset();
        RpcReset();
    }

    [ClientRpc]
    void RpcReset()
    {
        if (isServer)
            return;

        Reset();
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

    [ClientRpc]
    public void RpcDisableEffect(string actionName, string effectID, bool isManual)
    {
        if (isServer)
            return;

        var action = FindAction(actionName);
        var effect = action.FindEffect(effectID, isManual);

        action.DisableEffect(effect);
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

    bool isMine()
    {
        return player == game.localPlayer;
    }

    public void AddEffect(Effect.Base effect, string actionName, bool isManual = false)
    {
        var action = FindAction(actionName);
        action.AddEffect(effect, isManual);
    }

    [Client]
    public void OnPointerDown(PointerEventData data)
    {
        // invalidate click on cards outside card list
        if (game.discardPileList.gameObject.activeSelf && location != CardLocation.DISCARD_PILE)
            return;

        if (location == CardLocation.DISCARD_PILE && !game.discardPileList.gameObject.activeSelf)
        {
            game.ShowDiscardCards(player);
            return;
        }

        // click other player's hand card
        if (
            game.activePlayer != game.localPlayer ||
            (location == CardLocation.HAND && player != game.localPlayer)
        )
        {
            return;
        }

        if (
           game.state == GameState.CHOOSE_CARD &&
           game.localPlayer.IsOurTurn()
        )
        {
            game.ChooseCard(this);
            return;
        }


        // probably attacking a base
        if (
            game.state == GameState.DO_BASIC &&
            location == CardLocation.PLAY_AREA &&
            game.localPlayer != player &&
            type == CardType.BASE
        )
        {
            game.localPlayer.CmdAttackBase(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == CardLocation.PLAY_AREA
        )
        {
            game.ShowEffectList(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == CardLocation.HAND
        )
        {
            player.CmdPlayCard(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == CardLocation.TRADE_ROW
        )
        {
            game.localPlayer.CmdBuyCard(this);
            return;
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (location == CardLocation.DISCARD_PILE) { return; }

        transform.localScale = new Vector2(1.2f, 1.2f);
        image.sortingOrder = 2;
        border.sortingOrder = 1;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (location == CardLocation.DISCARD_PILE) { return; }

        transform.localScale = Vector2.one;
        image.sortingOrder = 0;
        border.sortingOrder = -1;
    }
}