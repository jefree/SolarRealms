using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class Hand : MonoBehaviour
{
    List<Card> cards = new();

    public void AddCard(Card card)
    {
        card.transform.SetParent(transform, false);
        card.gameObject.SetActive(true);

        card.location = Location.HAND;
        cards.Add(card);

        if (!card.player.IsLocalPlayer())
        {
            //card.Hide();
        }

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
