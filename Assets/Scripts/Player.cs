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
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;

    public Game game;
    public GameObject cardPrefab;

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
    public Stack<Card> deck;

    public TMPro.TextMeshProUGUI authorityScoreText;
    public TMPro.TextMeshProUGUI tradeScoreText;
    public TMPro.TextMeshProUGUI combatScoreText;

    // Start is called before the first frame update
    void Start()
    {
        discardPile = transform.Find("DiscardPile").GetComponent<DiscardPile>();
        playArea = transform.Find("PlayArea").GetComponent<PlayArea>();
        hand = transform.Find("Hand").GetComponent<Hand>();

        deck = generateInitialCards();

        combat = 0;
        trade = 50;
        authority = 50;
        authorityScoreText.text = $"{authority}";

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

        if (deck.Count == 0) { return; }

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
        if (deck.Count > 0)
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

    Stack<Card> generateInitialCards()
    {
        /* 
         create initial cards for testing
         turn this into actual creation logic when must of 
         gameplay is ready to avoid huge manual creation
       */

        Stack<Card> deck = new();

        for (int i = 0; i < INITIAL_SCOUT_AMOUNT; i++)
        {
            deck.Push(CardFactory.GenerateCard("scout", game, cardPrefab, this.gameObject, player: this));
        }

        for (int i = 0; i < INITIAL_VIPER_AMOUNT; i++)
        {
            deck.Push(CardFactory.GenerateCard("viper", game, cardPrefab, this.gameObject, player: this));
        }

        deck.Push(CardFactory.GenerateCard("infested moon", game, cardPrefab, this.gameObject, player: this));
        deck.Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: this));
        deck.Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: this));

        return deck;


    }
}
