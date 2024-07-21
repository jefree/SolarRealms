using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayArea : MonoBehaviour
{

    List<Card> cards = new();

    public void AddCard(Card card)
    {
        card.location = Location.PLAY_AREA;

        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(cards.Count * Game.CARD_SIZE, 0, 0);

        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        card.transform.SetParent(null);
        card.location = Location.UNDEFINED;

        cards.Remove(card);
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
