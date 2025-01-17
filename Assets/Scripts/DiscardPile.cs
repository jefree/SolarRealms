using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class DiscardPile : NetworkBehaviour
{

    public readonly SyncListCard cards = new();
    Card topCard;

    // Start is called before the first frame update
    void Start()
    {
        cards.OnChange += OnUpdateCards;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Client]
    void OnUpdateCards(SyncListCard.Operation op, int index, Card card)
    {
        switch (op)
        {
            case SyncList<Card>.Operation.OP_ADD:
                OnCardAdded(card);
                break;
            case SyncList<Card>.Operation.OP_REMOVEAT:
                OnCardRemoved(card);
                break;
        }

    }

    [Server]
    public void AddCard(Card card)
    {
        card.location = CardLocation.DISCARD_PILE;
        cards.Add(card);
    }

    [Client]
    public void OnCardAdded(Card card)
    {
        if (topCard != null)
        {
            topCard.transform.localPosition = new Vector2(-10f - 1.5f * cards.Count, 0f);
        }

        topCard = card;
        topCard.transform.SetParent(transform);
        topCard.transform.localPosition = Vector2.zero;
        topCard.transform.rotation = Quaternion.Euler(0, 0, 0);
        topCard.transform.localScale = Vector2.one;
    }

    [Server]
    public void RemoveCard(Card card)
    {
        card.location = CardLocation.UNDEFINED;
        cards.Remove(card);
    }

    [Client]
    void OnCardRemoved(Card card)
    {
        if (card == topCard)
            topCard = null;
    }

    [Server]
    public List<Card> RemoveAllCards()
    {
        var removedCards = new List<Card>();

        while (cards.Count > 0)
        {
            var card = cards.First();
            RemoveCard(card);
            removedCards.Add(card);
        }

        return removedCards;
    }

    [Server]
    public int Count()
    {
        return cards.Count;
    }
}
