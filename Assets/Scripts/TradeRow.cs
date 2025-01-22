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

    public override void OnStartServer()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public override void OnStartClient()
    {
        cards.OnChange += OnUpdateCards;

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

    void OnUpdateCards(SyncListCard.Operation op, int index, Card card)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_INSERT:
                OnCardInserted(card, index);
                break;

            case SyncListCard.Operation.OP_REMOVEAT:
                OnCardRemoved(card, index);
                break;
        }
    }

    [Server]
    public void CreateTradeDeck()
    {

        AddToTradeDeck("blob miner", 3);
        AddToTradeDeck("infested moon", 2);
        /*
        AddToTradeDeck("enforcer mech");
        AddToTradeDeck("frontier hawk", 3);
        AddToTradeDeck("gateship");
        AddToTradeDeck("hive queen");
        AddToTradeDeck("integration port", 2);
        AddToTradeDeck("neural nexus");
        AddToTradeDeck("outland station", 3);
        AddToTradeDeck("orbital shuttle", 3);
        AddToTradeDeck("pulverizer", 1);
        AddToTradeDeck("reclamation station");
        AddToTradeDeck("warpgate cruiser");
        AddToTradeDeck("repair mech", 2);
        */


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
        card.location = CardLocation.TRADE_ROW;
        cards.Insert(position, card);
    }

    void OnCardInserted(Card card, int position)
    {
        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(position * (Game.CARD_WIDTH + Game.CARD_PADDING), 0, 0);
    }

    public void RemoveCard(Card card)
    {
        card.location = CardLocation.UNDEFINED;

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
    void AddToTradeDeck(string cardName, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            var card = CardFactory.FromSO(cardName, game, cardPrefab, gameObject);
            //var card = CardFactory.GenerateCard(cardName, game, cardPrefab, gameObject);

            card.location = CardLocation.TRADE_DECK;
            tradeDeck.Insert(0, card);
            RpcCardAdded(card);

        }
    }

    [ClientRpc]
    void RpcCardAdded(Card card)
    {
        card.transform.position = new Vector2(20f, 0f);
    }
}
