using System.Collections.Generic;
using UnityEngine;

public class DiscardPileList : MonoBehaviour
{
    Transform panel;
    DiscardPile discardPile;
    List<Card> cards;

    public void Start()
    {

    }

    public void Show(DiscardPile discardPile)
    {
        gameObject.SetActive(true);

        Init();

        this.discardPile = discardPile;

        cards = discardPile.RemoveAllCards();

        foreach (var card in cards)
        {
            card.location = Location.DISCARD_PILE;
            card.gameObject.SetActive(true);
            card.transform.SetParent(panel);
            card.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }

    }

    public void Close()
    {
        foreach (var card in cards)
        {
            discardPile.AddCard(card);
        }

        cards = null;
        discardPile = null;
        gameObject.SetActive(false);
    }

    void Init()
    {
        if (panel)
        {
            return;
        }

        panel = transform.Find("Panel").transform;
    }
}