using System.Collections.Generic;
using UnityEngine;

public class DiscardPileList : MonoBehaviour
{
    Transform panel;
    DiscardPile discardPile;

    public void Show(DiscardPile discardPile)
    {
        gameObject.SetActive(true);

        Init();

        this.discardPile = discardPile;

        foreach (var card in discardPile.cards)
        {
            card.transform.SetParent(panel);
            card.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }

    public void Close()
    {
        foreach (var card in discardPile.cards)
        {
            discardPile.OnCardAdded(card);
        }

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