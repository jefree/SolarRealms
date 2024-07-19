using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Effect primaryMainEffect;
    public Effect primaryOptionalEffect;
    public Effect allyEffect;
    public Effect doubleAllyEffect;
    public Effect scrapEffect;

    string secondaryText; // ??
    public int cost;
    public int defense;
    public bool outpost;
    public Faction faction; // Unaligned, Trade Federation, The blobs, Star Empire, Machine Cult
    public int type; // Ship or Base
    public Game game;
    public Location location;


    public void Start()
    {

    }
    void OnMouseDown()
    {
        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.HAND
        )
        {
            Debug.Log("Playing Card");
            game.PlayCard(this);
        }

        if (
            game.state == GameState.PLAY_CARD &&
            location == Location.TRADE_ROW
        )
        {
            Debug.Log("Buying Card");
            game.BuyCard(this);
        }
    }
}