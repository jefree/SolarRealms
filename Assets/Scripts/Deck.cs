using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Deck : MonoBehaviour
{

    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;

    public Game game;
    public Player player;
    public GameObject cardPrefab;

    public TMPro.TextMeshProUGUI countText;

    [HideInInspector]
    public Stack<Card> cards;

    void Start()
    {
        game = player.game;
        cards = generateInitialCards();
    }

    public int Count()
    {
        return cards.Count;
    }

    public void Clear()
    {
        cards.Clear();
    }

    public void Push(Card card)
    {
        cards.Push(card);
        UpdateCountText();
    }

    public Card Pop()
    {
        var card = cards.Pop();
        UpdateCountText();

        return card;
    }

    void UpdateCountText()
    {
        countText.text = $"{cards.Count}";
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
            deck.Push(CardFactory.GenerateCard("scout", game, cardPrefab, this.gameObject, player: player));
        }

        for (int i = 0; i < INITIAL_VIPER_AMOUNT; i++)
        {
            deck.Push(CardFactory.GenerateCard("viper", game, cardPrefab, this.gameObject, player: player));
        }

        deck.Push(CardFactory.GenerateCard("infested moon", game, cardPrefab, this.gameObject, player: player));
        deck.Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: player));
        deck.Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: player));

        return deck;


    }
}
