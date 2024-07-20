using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRow : MonoBehaviour
{

    public Game game;
    public GameObject cardPrefab;
    [HideInInspector]
    List<Card> cards;

    // Start is called before the first frame update
    void Start()
    {
        cards = new();

        AddCard(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject), 0);
        AddCard(CardFactory.GenerateCard("stinger", game, cardPrefab, this.gameObject), 1);
        AddCard(CardFactory.GenerateCard("frontier runner", game, cardPrefab, this.gameObject), 2);
        AddCard(CardFactory.GenerateCard("blob miner", game, cardPrefab, this.gameObject), 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddCard(Card card, int position)
    {
        card.location = Location.TRADE_ROW;
        cards.Add(card);

        card.gameObject.transform.Translate(new Vector3(position * Game.CARD_SIZE, 0, 0));
        card.gameObject.SetActive(true);
    }

    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;
        card.gameObject.SetActive(false);
    }
}
