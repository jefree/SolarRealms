using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Effect;
using EffectResolver;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.SocialPlatforms;

public enum Faction
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

public class Card : MonoBehaviour
{
    public string cardName;
    public Action mainAction;
    public Action allyAction;
    public Action doubleAllyAction;
    public Action scrapAction;

    public int cost;
    public int defense;
    public bool outpost;
    public Faction faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    public CardType type; // Ship or Base
    public Game game;
    public Location location;
    public Player player;

    public GameObject combatUpPrefab;
    EffectUp effectUp;

    void Start()
    {
        effectUp = Instantiate(combatUpPrefab, transform).GetComponent<EffectUp>();
    }

    public void Activate()
    {
        var firstAction = NextAction();
        game.ResolveAction(firstAction);
    }

    public Action NextAction()
    {
        return ActualActions(mainAction, allyAction, doubleAllyAction).Find(action => action.HasPendingEffects());
    }

    public Action NextManualAction()
    {
        return ActualActions(mainAction, scrapAction).Find(action => !action.activated);
    }

    public void Reset()
    {
        ActualActions(mainAction, scrapAction).ForEach(action => action.Reset());
    }

    public List<Action> Actions()
    {
        return ActualActions(mainAction, scrapAction);
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

    public bool HasPendingActions()
    {
        return ActualActions(mainAction, allyAction, doubleAllyAction).Any(action => action.HasPendingEffects());
    }

    public void ShowEffect(EffectColor color, float value)
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

    void OnMouseDown()
    {

        if (location == Location.DISCARD_PILE)
        {
            game.ShowDiscardCards(player);
            return;
        }

        // probably attacking a base
        if (
            game.state == GameState.DO_BASIC &&
            location == Location.PLAY_AREA &&
            game.activePlayer != player &&
            type == CardType.BASE
        )
        {
            game.AttackBase(this);
        }

        if (player != null && game.activePlayer != player)
        {
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.PLAY_AREA &&
            NextManualAction() != null
        )
        {
            game.ChooseEffect(this);
            return;
        }

        if (
            game.state == GameState.CHOOSE_CARD
        )
        {
            game.ChooseCard(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.HAND
        )
        {
            game.PlayCard(this);
            return;
        }

        if (
            game.state == GameState.DO_BASIC &&
            location == Location.TRADE_ROW
        )
        {
            game.BuyCard(this);
            return;
        }
    }

    void OnMouseEnter()
    {

        if (location == Location.DISCARD_PILE) { return; }

        transform.localScale = new Vector2(1.5f, 1.5f);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    void OnMouseExit()
    {
        if (location == Location.DISCARD_PILE) { return; }

        transform.localScale = new Vector2(1f, 1f);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }
}