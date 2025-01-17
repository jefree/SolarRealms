using UnityEngine;

public class DiscardPileList : MonoBehaviour
{
    public Transform panel;
    public Canvas canvas;
    DiscardPile discardPile;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Show(DiscardPile discardPile)
    {

        gameObject.SetActive(true);

        this.discardPile = discardPile;

        foreach (var card in discardPile.cards)
        {
            card.transform.SetParent(panel);
            card.border.sortingOrder = canvas.sortingOrder + 1;
            card.image.sortingOrder = canvas.sortingOrder + 2;
        }
    }

    public void Close()
    {
        foreach (var card in discardPile.cards)
        {
            card.GetComponent<SpriteRenderer>().sortingOrder = 1;
            discardPile.OnCardAdded(card);
        }

        discardPile = null;
        gameObject.SetActive(false);
    }
}