using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Mirror;

public class Hand : NetworkBehaviour
{
    readonly SyncList<Card> cards = new();

    public override void OnStartClient()
    {
        cards.Callback += OnCardsUpdated;

        foreach (var card in cards)
        {
            OnCardAdded(card);
        }
    }

    private void OnCardsUpdated(SyncList<Card>.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncList<Card>.Operation.OP_ADD:
                OnCardAdded(newCard);
                break;
        }
    }

    // Server
    public void AddCard(Card card)
    {
        card.location = Location.HAND;
        cards.Add(card);

        OnCardAdded(card);
    }

    //Server & Client
    public void OnCardAdded(Card card)
    {
        card.transform.SetParent(transform, false);
        card.gameObject.SetActive(true);

        //if (!card.player.isLocalPlayer)
        //{
        //    card.Hide();
        //}

        RepositionCards();
    }

    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;
        card.transform.SetParent(null);

        cards.Remove(card);

        card.Show();

        RepositionCards();
    }

    void RepositionCards()
    {

        var i = 0;
        foreach (var card in cards)
        {
            card.transform.localPosition = new Vector3(0, 0, 0);
            card.transform.Translate(new Vector3(i * Game.CARD_SIZE, 0, 0), Space.World);


            i++;
        }

    }

    public int Count()
    {
        return cards.Count;
    }

    public Card FirstCard()
    {
        return cards.First<Card>();
    }
}
