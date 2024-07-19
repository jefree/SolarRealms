using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public const int INITIAL_HAND_SIZE = 5;
    public const float CARD_HAND_PADDING = 0.15f;

    [HideInInspector]
    public int combat;
    [HideInInspector]
    public int trade;
    [HideInInspector]
    public int authority;

    [HideInInspector]
    public Stack<Card> deck;
    [HideInInspector]
    public List<Card> hand;
    [HideInInspector]
    public Game game;
    [HideInInspector]
    public DiscardPile discardPile;

    public GameObject gameGO;
    public TMPro.TextMeshProUGUI authorityScoreText;
    public TMPro.TextMeshProUGUI tradeScoreText;
    public TMPro.TextMeshProUGUI combatScoreText;

    const int INITIAL_SCOUT_AMOUNT  = 8;
    const int INITIAL_VIPER_AMOUNT  = 2;
    public GameObject cardPrefab;
    public GameObject discardPileGO;

    // Start is called before the first frame update
    void Start()
    {   
        game = gameGO.GetComponent<Game>();
        deck = generateInitialCards();
        discardPile = discardPileGO.GetComponent<DiscardPile>();
        hand = new();

        combat = 0;
        trade = 0;
        authority = 50;
        
        for(int i=0; i < INITIAL_HAND_SIZE; i++) {
           DrawCard();
        }
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

    public void AddToHand(Card card)
    {
        card.gameObject.transform.Translate(new Vector3(hand.Count * Game.CARD_SIZE, 0, 0));
        card.gameObject.SetActive(true);

        card.location = Location.HAND;
        hand.Add(card);
    }

    public void Discard(Card card)
    {
        hand.Remove(card);
        card.gameObject.transform.SetParent(null);
        card.gameObject.SetActive(false);
        discardPile.AddCard(card);
    }

    public void BuyCard(Card card)
    {
        discardPile.AddCard(card);
    }

     Stack<Card> generateInitialCards() {
         /* 
          create initial cards for testing
          turn this into actual creation logic when must of 
          gameplay is ready to avoid huge manual creation
        */

        Stack<Card> deck = new();

        for(int i=0; i<INITIAL_SCOUT_AMOUNT; i++) {
            deck.Push(CardEffectFactory.GenerateCard("scout", game, cardPrefab, this.gameObject)); 
        }

        for(int i=0; i<INITIAL_VIPER_AMOUNT; i++) {
            deck.Push(CardEffectFactory.GenerateCard("viper", game, cardPrefab, this.gameObject)); 
        }

        // deck.Push(generateCard("frontier runner")); 

        return deck;
    }
}
