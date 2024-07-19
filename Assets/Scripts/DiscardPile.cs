using System.Collections;
using System.Collections.Generic;
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
            topCard.transform.localPosition = new Vector3(0, 0, 1);
        }

        topCard = card.gameObject;
        topCard.transform.SetParent(gameObject.transform);
        topCard.transform.localPosition = new Vector3(0, 0, 0);
        topCard.SetActive(true);

        card.location = Location.DISCARD_PILE;
        cards.Add(card);
    }
}
