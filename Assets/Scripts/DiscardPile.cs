using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class DiscardPile : NetworkBehaviour
{

    public readonly SyncListCard cards = new();
    GameObject topCard;

    // Start is called before the first frame update
    void Start()
    {
        cards.Callback += OnUpdateCards;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Client]
    void OnUpdateCards(SyncListCard.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncList<Card>.Operation.OP_ADD:
                OnCardAdded(newCard);
                break;
            case SyncList<Card>.Operation.OP_REMOVEAT:
                OnCardRemoved(oldCard);
                break;
        }

    }

    [Server]
    public void AddCard(Card card)
    {
        card.location = Location.DISCARD_PILE;
        cards.Add(card);
    }

    [Client]
    void OnCardAdded(Card card)
    {
        if (topCard != null)
        {
            topCard.transform.position = new Vector2(100f, 100f);
        }

        topCard = card.gameObject;
        topCard.transform.SetParent(transform);
        topCard.transform.localPosition = Vector2.zero;
        topCard.transform.rotation = Quaternion.Euler(0, 0, 0);
        topCard.transform.localScale = Vector2.one;
    }

    [Server]
    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;
        cards.Remove(card);
    }

    [Client]
    void OnCardRemoved(Card card)
    {
        card.transform.SetParent(null);
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
