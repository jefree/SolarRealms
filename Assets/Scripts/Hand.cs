using System.Linq;
using UnityEngine;
using Mirror;

public class Hand : NetworkBehaviour
{
    readonly SyncListCard cards = new();

    public override void OnStartClient()
    {

        cards.Callback += OnUpdateCards;

        foreach (var card in cards)
        {
            OnCardAdded(card);
        }

    }

    void OnUpdateCards(SyncListCard.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_ADD:
                OnCardAdded(newCard);
                break;
            case SyncListCard.Operation.OP_REMOVEAT:
                OnCardRemoved(oldCard);
                break;
        }
    }

    [Server]
    public void AddCard(Card card)
    {
        card.location = Location.HAND;
        cards.Add(card);
    }

    [Client]
    void OnCardAdded(Card card)
    {
        card.transform.SetParent(transform);

        if (!card.player.isLocalPlayer)
        {
            card.Hide();
        }

        RepositionCards();
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
        if (!card.player.isLocalPlayer)
            card.Show();

        RepositionCards();
    }

    void RepositionCards()
    {

        var i = 0;
        foreach (var card in cards)
        {
            card.transform.localPosition = new Vector3(0, 0, 0);
            card.transform.Translate(new Vector3(i * (Game.CARD_WIDTH + Game.CARD_PADDING), 0, 0), Space.World);

            i++;
        }
    }

    public int Count()
    {
        return cards.Count;
    }

    public Card FirstCard()
    {
        return cards.First();
    }
}
