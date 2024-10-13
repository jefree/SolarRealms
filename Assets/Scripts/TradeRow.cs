using System.Collections;
using System.Collections.Generic;
using Mirror;
using Org.BouncyCastle.Asn1.Cmp;
using Unity.VisualScripting;
using UnityEngine;

public class TradeRow : NetworkBehaviour
{

    public Game game;
    public GameObject cardPrefab;
    public int tradeRowSize = 5;
    [HideInInspector] List<Card> tradeDeck = new();
    [HideInInspector] readonly SyncListCard cards = new();

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public override void OnStartClient()
    {
        cards.Callback += OnUpdateCards;

        int i = 0;
        foreach (var card in cards)
        {
            OnCardInserted(card, i);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnUpdateCards(SyncListCard.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_INSERT:
                OnCardInserted(newCard, index);
                break;

            case SyncListCard.Operation.OP_REMOVEAT:
                OnCardRemoved(oldCard, index);
                break;
        }
    }

    [Server]
    public void CreateTradeDeck()
    {
        AddToTradeDeck(CardFactory.GenerateCard("hive queen", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("infested moon", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("enforcer mech", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("integration port", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("integration port", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("hive queen", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("infested moon", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("enforcer mech", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));
        AddToTradeDeck(CardFactory.GenerateCard("blob miner", game, cardPrefab, gameObject));

        Util.Shuffle(tradeDeck);
    }

    [Server]
    public void Init()
    {
        for (var i = 0; i < tradeRowSize; i++)
        {
            AddCard(tradeDeck[0], i);
            tradeDeck.RemoveAt(0);
        }
    }

    [Server]
    void AddCard(Card card, int position)
    {
        card.location = Location.TRADE_ROW;
        cards.Insert(position, card);
    }

    void OnCardInserted(Card card, int position)
    {
        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(position * Game.CARD_SIZE, 0, 0);
    }

    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;

        var freePosition = cards.IndexOf(card);
        cards.Remove(card);

        if (tradeDeck.Count > 0)
        {

            AddCard(tradeDeck[0], freePosition);
            tradeDeck.RemoveAt(0);
        }
    }

    void OnCardRemoved(Card card, int position)
    {
        // empty ??
    }

    [Server]
    public void ScrapCard(Card card)
    {
        RemoveCard(card);
    }

    [Server]
    void AddToTradeDeck(Card card)
    {
        card.location = Location.TRADE_DECK;
        tradeDeck.Insert(0, card);
        RpcCardAdded(card);
    }

    [ClientRpc]
    void RpcCardAdded(Card card)
    {
        card.transform.position = new Vector2(20f, 0f);
    }
}
