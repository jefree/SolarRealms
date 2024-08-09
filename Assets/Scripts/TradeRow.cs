using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TradeRow : MonoBehaviour
{

    public Game game;
    public GameObject cardPrefab;
    [HideInInspector]
    Stack<Card> tradeDeck = new();
    [HideInInspector]
    List<Card> cards = new();

    // Start is called before the first frame update
    void Start()
    {
        AddCard(CardFactory.GenerateCard("hive queen", game, cardPrefab, gameObject), 0);
        AddCard(CardFactory.GenerateCard("infested moon", game, cardPrefab, gameObject), 1);
        AddCard(CardFactory.GenerateCard("enforcer mech", game, cardPrefab, gameObject), 2);
        AddCard(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject), 3);
        AddCard(CardFactory.GenerateCard("integration port", game, cardPrefab, gameObject), 4);

        tradeDeck.Push(CardFactory.GenerateCard("integration port", game, cardPrefab, gameObject));
        tradeDeck.Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, gameObject));
        tradeDeck.Push(CardFactory.GenerateCard("infested moon", game, cardPrefab, gameObject));
        tradeDeck.Push(CardFactory.GenerateCard("enforcer mech", game, cardPrefab, gameObject));
        tradeDeck.Push(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddCard(Card card, int position)
    {
        card.location = Location.TRADE_ROW;
        cards.Insert(position, card);

        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(position * Game.CARD_SIZE, 0, 0);
        card.gameObject.SetActive(true);
    }

    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;
        card.gameObject.SetActive(false);

        var freePosition = cards.IndexOf(card);
        cards.Remove(card);

        if (tradeDeck.Count > 0)
        {
            AddCard(tradeDeck.Pop(), freePosition);
        }
    }
}
