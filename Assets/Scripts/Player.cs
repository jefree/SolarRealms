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
    public DiscardPile discardPile;
    public GameObject cardPrefab;
    public GameObject playAreaGO;

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
    [HideInInspector]
    public List<Card> hand = new();
    [HideInInspector]
    public List<Card> playArea = new();


    public TMPro.TextMeshProUGUI authorityScoreText;
    public TMPro.TextMeshProUGUI tradeScoreText;
    public TMPro.TextMeshProUGUI combatScoreText;

    // Start is called before the first frame update
    void Start()
    {
        deck = generateInitialCards();

        combat = 0;
        trade = 0;
        authority = 50;

        DrawNewHand();
    }

    // Update is called once per frame
    void Update()
    {
        authorityScoreText.text = $"{authority}";
        tradeScoreText.text = $"{trade}";
        combatScoreText.text = $"{combat}";
    }

    public void DrawCard()
    {
        var card = deck.Pop();
        AddToHand(card);
    }

    public void DrawNewHand()
    {

        if (deck.Count < INITIAL_HAND_SIZE)
        {
            ShuffleDiscard();
        }

        for (int i = 0; i < INITIAL_HAND_SIZE; i++)
        {
            DrawCard();
        }
    }

    public void ShuffleDiscard()
    {
        var cards = discardPile.RemoveAllCards();
        var remainingCards = deck.ToList<Card>();

        deck.Clear();

        foreach (var card in cards)
        {
            deck.Push(card);
            card.gameObject.transform.SetParent(gameObject.transform);
        }

        foreach (var card in remainingCards)
        {
            deck.Push(card);
            card.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    public void AddToHand(Card card)
    {
        card.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        card.gameObject.transform.Translate(new Vector3(hand.Count * Game.CARD_SIZE, 0, 0));
        card.gameObject.SetActive(true);

        card.location = Location.HAND;
        hand.Add(card);
    }

    public void PlayCard(Card card)
    {
        hand.Remove(card);
        playArea.Add(card);

        card.location = Location.PLAY_AREA;

        card.transform.SetParent(playAreaGO.transform);
        card.transform.localPosition = new Vector3(cardsInPlay * Game.CARD_SIZE, 0, 0);

        cardsInPlay += 1;
    }

    public void Discard(Card card)
    {
        playArea.Remove(card);
        card.transform.SetParent(null);
        card.gameObject.SetActive(false);
        discardPile.AddCard(card);
    }

    public bool CanBuyCard(Card card)
    {
        return trade >= card.cost;
    }

    public void BuyCard(Card card)
    {
        trade -= card.cost;


        discardPile.AddCard(card);
    }

    public void EndTurn()
    {
        combat = 0;
        trade = 0;

        while (hand.Count > 0)
        {
            Discard(hand.First<Card>());
        }

        DrawNewHand();
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
            deck.Push(CardFactory.GenerateCard("scout", game, cardPrefab, this.gameObject));
        }

        for (int i = 0; i < INITIAL_VIPER_AMOUNT; i++)
        {
            deck.Push(CardFactory.GenerateCard("viper", game, cardPrefab, this.gameObject));
        }

        deck.Push(CardFactory.GenerateCard("blob miner", game, cardPrefab, this.gameObject));
        deck.Push(CardFactory.GenerateCard("blob miner", game, cardPrefab, this.gameObject));

        // deck.Push(generateCard("frontier runner")); 

        return deck;
    }
}
