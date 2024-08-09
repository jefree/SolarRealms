using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{

    [HideInInspector]
    List<Card> cards;
    [HideInInspector]
    public GameObject topCard;

    // Start is called before the first frame update
    void Start()
    {
        cards = new();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(Card card)
    {
        if (topCard != null)
        {
            topCard.SetActive(false);
        }

        topCard = card.gameObject;
        topCard.transform.SetParent(gameObject.transform);
        topCard.transform.localPosition = new Vector3(0, 0, 0);
        topCard.transform.rotation = new Quaternion();
        topCard.SetActive(true);

        card.location = Location.DISCARD_PILE;
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        card.transform.SetParent(null);

        card.location = Location.UNDEFINED;
        cards.Remove(card);
    }

    public List<Card> RemoveAllCards()
    {
        var removedCards = new List<Card>();

        while (cards.Count > 0)
        {
            var card = cards.First<Card>();
            RemoveCard(card);
            removedCards.Add(card);
        }

        return removedCards;
    }

    public int Count()
    {
        return cards.Count;
    }
}
