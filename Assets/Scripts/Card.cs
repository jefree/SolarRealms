using System;
using System.Collections;
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

public class Card : MonoBehaviour
{
    public string cardName;
    public Action mainAction;
    // public List<Action> allyActions;
    // public List<Action> doubleAllyActions;
    public Action scrapAction;

    string secondaryText; // ??
    public int cost;
    public int defense;
    public bool outpost;
    public Faction faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    public int type; // Ship or Base
    public Game game;
    public Location location;
    public Player player;

    public void Play()
    {
        var firstAction = NextAction();
        game.ResolveAction(firstAction);
    }

    public Action NextAction()
    {
        return Actions(mainAction).Find(action => !action.activated);
    }

    public Action NextManualAction()
    {
        return Actions(mainAction, scrapAction).Find(action => !action.activated);
    }

    public void Reset()
    {
        Actions(mainAction, scrapAction).ForEach(action => action.Reset());
    }

    List<Action> Actions(params Action[] actions)
    {
        var validActions = actions.ToList();
        validActions.RemoveAll(action => action == null);

        if (validActions.Count == 0)
        {
            throw new InvalidOperationException($"card with no actions '{cardName}'");
        }

        return validActions;
    }

    void OnMouseDown()
    {

        if (player != null && game.activePlayer != player)
        {
            return;
        }

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.PLAY_AREA &&
            NextManualAction() != null
        )
        {
            game.ChooseEffect(this);
        }

        if (
            game.state == GameState.CHOOSE_CARD
        )
        {
            game.ChooseCard(this);
            return;
        }

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.HAND
        )
        {
            game.PlayCard(this);
            return;
        }

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.TRADE_ROW
        )
        {
            game.BuyCard(this);
            return;
        }
    }
}