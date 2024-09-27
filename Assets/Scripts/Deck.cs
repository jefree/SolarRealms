using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class Deck : NetworkBehaviour
{

    const int INITIAL_SCOUT_AMOUNT = 8;
    const int INITIAL_VIPER_AMOUNT = 2;

    public Game game;
    public Player player;
    public GameObject cardPrefab;

    public TMPro.TextMeshProUGUI countText;

    [HideInInspector]
    public readonly SyncList<Card> cards = new();

    public override void OnStartClient()
    {
        cards.Callback += OnUpdateCards;

        foreach (var card in cards)
        {
            OnCardInserted(card);
        }
    }

    void OnUpdateCards(SyncList<Card>.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncList<Card>.Operation.OP_INSERT:
                OnCardInserted(newCard);
                break;
        }

    }

    // Server & Client
    void OnCardInserted(Card card)
    {
        card.transform.SetParent(transform);
        card.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{card.cardName}");
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

    // Server
    public void Push(Card card)
    {
        cards.Insert(0, card);
        UpdateCountText();

        OnCardInserted(card);
    }

    public Card Pop()
    {
        var card = cards[0];
        cards.RemoveAt(0);

        UpdateCountText();

        return card;
    }

    void UpdateCountText()
    {
        countText.text = $"{cards.Count}";
    }

    // Server
    void GenerateInitialCards()
    {
        /* 
         create initial cards for testing
         turn this into actual creation logic when must of 
         gameplay is ready to avoid huge manual creation
       */

        for (int i = 0; i < INITIAL_SCOUT_AMOUNT; i++)
        {
            Push(CardFactory.GenerateCard("scout", game, cardPrefab, this.gameObject, player: player));
        }

        for (int i = 0; i < INITIAL_VIPER_AMOUNT; i++)
        {
            Push(CardFactory.GenerateCard("viper", game, cardPrefab, this.gameObject, player: player));
        }

        Push(CardFactory.GenerateCard("infested moon", game, cardPrefab, this.gameObject, player: player));
        Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: player));
        Push(CardFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject, player: player));
    }
}
