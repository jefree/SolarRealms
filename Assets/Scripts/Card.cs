using System.Collections;
using System.Collections.Generic;
using Effect;
using EffectResolver;
using UnityEngine;
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

    public void Play()
    {

        var firstAction = NextAction();
        firstAction.Activate();
    }

    public Action NextAction()
    {
        return mainAction;
    }

    void OnMouseDown()
    {

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.PLAY_AREA
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
            Debug.Log("Playing Card");
            game.PlayCard(this);
            return;
        }

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.TRADE_ROW
        )
        {
            Debug.Log("Buying Card");
            game.BuyCard(this);
            return;
        }
    }
}