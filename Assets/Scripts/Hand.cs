using System.Linq;
using UnityEngine;
using Mirror;

public class Hand : NetworkBehaviour
{
    readonly SyncListCard cards = new();

    public override void OnStartClient()
    {

        cards.OnChange += OnUpdateCards;

        foreach (var card in cards)
        {
            OnCardAdded(card);
        }

    }

    void OnUpdateCards(SyncListCard.Operation op, int index, Card card)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_ADD:
                OnCardAdded(card);
                break;
            case SyncListCard.Operation.OP_REMOVEAT:
                OnCardRemoved(card);
                break;
        }
    }

    [Server]
    public void AddCard(Card card)
    {
        card.location = CardLocation.HAND;
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
        card.location = CardLocation.UNDEFINED;
        cards.Remove(card);
    }

    [Client]
    public void OnCardRemoved(Card card, int diff = 0)
    {
        if (!card.player.isLocalPlayer)
            card.Show();

        RepositionCards(diff);
    }

    void RepositionCards(int diff = 0)
    {
        var i = 0;
        foreach (var card in cards)
        {
            card.transform.localPosition = new Vector3(0, 0, 0);
            card.transform.Translate(new Vector3((i - diff) * (Game.CARD_WIDTH + Game.CARD_PADDING), 0, 0), Space.World);

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
