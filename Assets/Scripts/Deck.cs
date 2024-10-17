using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Org.BouncyCastle.Asn1.Cmp;

public class Deck : NetworkBehaviour
{
    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;

    public Game game;
    public Player player;
    public GameObject cardPrefab;

    public TMPro.TextMeshProUGUI countText;

    [HideInInspector]
    public readonly SyncListCardInfo cards = new();

    public override void OnStartClient()
    {
        cards.Callback += OnUpdateCards;

        foreach (var card in cards)
        {
            OnCardInserted(card);
        }
    }

    void OnUpdateCards(SyncListCardInfo.Operation op, int index, CardInfo oldCard, CardInfo newCard)
    {
        switch (op)
        {
            case SyncListCardInfo.Operation.OP_INSERT:
                OnCardInserted(newCard);
                break;

            case SyncListCardInfo.Operation.OP_REMOVEAT:
                OnCardRemoved(oldCard);
                break;

        }

        UpdateCountText();

    }

    [Client]
    public void OnCardInserted(CardInfo info)
    {
        Util.PopulateCardInfo(ref info);

        var card = info.card;

        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector2(10f + 1.5f * cards.Count, 0f);
    }

    [Client]
    void OnCardRemoved(CardInfo info)
    {
        /* if (info.card == null)
            return; */


        //info.card.transform.SetParent(null);
        //info.card.Show();
    }

    public void Init(Player player)
    {
        this.player = player;
        game = player.game;

        GenerateInitialCards();
    }

    public int Count()
    {
        return cards.Count;
    }

    public void Clear()
    {
        cards.Clear();
    }

    [Server]
    public void Push(Card card)
    {
        card.location = Location.DECK;
        var cardInfo = new CardInfo(card);

        card.NetReset();
        cards.Insert(0, cardInfo);
    }

    [Server]
    public Card Pop()
    {
        var info = cards[0];
        cards.RemoveAt(0);

        return info.card;
    }

    [Client]
    void UpdateCountText()
    {
        countText.text = $"{cards.Count}";
    }

    [Server]
    void GenerateInitialCards()
    {
        /* 
         create initial cards for testing
         turn this into actual creation logic when must of 
         gameplay is ready to avoid huge manual creation
       */

        List<Card> initial = new();

        for (int i = 0; i < INITIAL_SCOUT_AMOUNT; i++)
        {
            initial.Add(CardFactory.GenerateCard("scout", game, cardPrefab, this.gameObject, player: player));
        }

        for (int i = 0; i < INITIAL_VIPER_AMOUNT; i++)
        {
            initial.Add(CardFactory.GenerateCard("viper", game, cardPrefab, this.gameObject, player: player));
        }

        initial.Add(CardFactory.GenerateCard("outland station", game, cardPrefab, gameObject, player: player));
        //initial.Add(CardFactory.GenerateCard("outland station", game, cardPrefab, gameObject, player: player));

        Util.Shuffle(initial);

        foreach (var card in initial)
        {
            Push(card);
        }
    }
}
