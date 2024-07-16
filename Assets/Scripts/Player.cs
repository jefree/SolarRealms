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

    public int combat;
    public int trade;
    public int authority;

    public Stack<Card> deck;
    public Stack<Card> discardPile;
    public List<Card> hand;
    public Game game;
    public TMPro.TextMeshProUGUI authorityScoreText;
    public TMPro.TextMeshProUGUI tradeScoreText;
    public TMPro.TextMeshProUGUI combatScoreText;

    const int INITIAL_SCOUT_AMOUNT  = 8;
    const int INITIAL_VIPER_AMOUNT  = 2;
    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {   

        deck = generateInitialCards();
        hand = new();

        combat = 0;
        trade = 0;
        authority = 50;
        
        for(int i=0; i < INITIAL_HAND_SIZE; i++) {
            hand.Add(deck.Pop());
        }

        float cardSize = hand.First<Card>().gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        float halfHandSize = (hand.Count * cardSize) / 2;

        for(int i=0; i < hand.Count; i++)  {
            Card card = hand[i];

            card.gameObject.transform.position = new Vector3(-halfHandSize + i*cardSize, 0, 0);
            card.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        authorityScoreText.text = $"{authority}";
        tradeScoreText.text = $"{trade}";
        combatScoreText.text = $"{combat}";
    }

     Stack<Card> generateInitialCards() {
         /* 
          create initial cards for testing
          turn this into actual creation logic when must of 
          gameplay is ready to avoid huge manual creation
        */

        Stack<Card> deck = new();

        for(int i=0; i<INITIAL_SCOUT_AMOUNT; i++) {
            deck.Push(generateCard("scout")); 
        }

        for(int i=0; i<INITIAL_VIPER_AMOUNT; i++) {
            deck.Push(generateCard("viper")); 
        }

        // deck.Push(generateCard("frontier runner")); 

        return deck;
    }

    Card generateCard(string name) {
        GameObject cardGameObject = Instantiate(cardPrefab);

        Card card = cardGameObject.GetComponent<Card>();
        card.cardName = name;

        Effect cardEffect = new Effect();
        cardEffect.combat = 1;
        card.primaryMainEffect = cardEffect;

        card.gameObject.SetActive(false);
        card.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{name}");

        return card;
    }
}
