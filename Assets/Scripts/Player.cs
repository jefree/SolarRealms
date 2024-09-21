using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public const int INITIAL_HAND_SIZE = 5;
    public const float CARD_HAND_PADDING = 0.15f;

    public Game game;

    [HideInInspector]
    public Deck deck;
    [HideInInspector]
    public Hand hand;
    [HideInInspector]
    public PlayArea playArea;
    [HideInInspector]
    public DiscardPile discardPile;
    [HideInInspector]
    public int combat;
    [HideInInspector]
    public int trade;
    [HideInInspector]
    public int authority;
    [HideInInspector]
    public int cardsInPlay;


    [HideInInspector]
    public TMPro.TextMeshProUGUI authorityScoreText;
    [HideInInspector]
    public TMPro.TextMeshProUGUI tradeScoreText;
    [HideInInspector]
    public TMPro.TextMeshProUGUI combatScoreText;

    // Start is called before the first frame update
    void Start()
    {
        deck = transform.Find("Deck").GetComponent<Deck>();
        discardPile = transform.Find("DiscardPile").GetComponent<DiscardPile>();
        playArea = transform.Find("PlayArea").GetComponent<PlayArea>();
        hand = transform.Find("Hand").GetComponent<Hand>();

        tradeScoreText = GameObject.Find("TradeText").GetComponent<TMPro.TextMeshProUGUI>();
        combatScoreText = GameObject.Find("CombatText").GetComponent<TMPro.TextMeshProUGUI>();

        combat = 0;
        trade = 0;
        authority = 50;
        authorityScoreText.text = $"{authority}";

        Debug.Log("Start Player");

        DrawNewHand();
    }

    // Update is called once per frame
    void Update()
    {
        authorityScoreText.text = $"{authority}";

        if (game.activePlayer != this)
        {
            return;
        }

        tradeScoreText.text = $"{trade}";
        combatScoreText.text = $"{combat}";
    }

    public void DrawCard()
    {
        ShuffleDiscard();

        if (deck.Count() == 0) { return; }

        var card = deck.Pop();
        hand.AddCard(card);
    }

    public void DrawNewHand()
    {
        for (int i = 0; i < INITIAL_HAND_SIZE; i++)
        {
            DrawCard();
        }
    }

    public void ShuffleDiscard()
    {
        if (deck.Count() > 0)
        {
            return;
        }

        var cards = discardPile.RemoveAllCards();
        deck.Clear();

        foreach (var card in cards)
        {
            card.Reset();
            card.location = Location.DECK;
            deck.Push(card);
            card.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    public void PlayCard(Card card)
    {
        if (card.location == Location.HAND)
        {
            hand.RemoveCard(card);
            playArea.AddCard(card);
        }
    }

    public bool CanBuyCard(Card card)
    {
        return trade >= card.cost;
    }

    public void BuyCard(Card card)
    {
        trade -= card.cost;

        card.player = this;
        discardPile.AddCard(card);
    }

    public void ScrapCard(Card card)
    {
        if (card.location == Location.HAND)
        {
            hand.RemoveCard(card);
            return;
        }

        throw new InvalidOperationException("location invalida para deshuesar");
    }

    public void StartTurn()
    {
        playArea.ActivateBases();
    }

    public void EndTurn()
    {
        combat = 0;
        trade = 0;

        //TODO: improve to let hand handle this removing logic
        while (hand.Count() > 0)
        {
            var card = hand.FirstCard();
            hand.RemoveCard(card);
            discardPile.AddCard(card);
        }


        var discardedShips = playArea.DiscardShips();
        discardedShips.ForEach(card => discardPile.AddCard(card));

        playArea.ResetBases();

        DrawNewHand();
    }

    public void Attacked()
    {
        game.AttackPlayer(this);
    }

    public bool HasOutpost()
    {
        return playArea.HasOutpost();
    }

    public void SpendCombat(int value)
    {
        combat -= value;
    }

    public void DestroyBase(Card card)
    {
        playArea.DestroyBase(card);
        discardPile.AddCard(card);
    }

    public bool IsLocalPlayer()
    {
        return game.localPlayer == this;
    }
}

