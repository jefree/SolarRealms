using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRow : MonoBehaviour
{

    public GameObject gameGO;

    [HideInInspector]
    public GameObject cardPrefab;
    [HideInInspector]
    public Game game;
    [HideInInspector]
    List<Card> cards;

    // Start is called before the first frame update
    void Start()
    {

        game = gameGO.GetComponent<Game>();
        cards = new();

        AddCard(CardEffectFactory.GenerateCard("hive queen", game, cardPrefab, this.gameObject), 0);
        AddCard(CardEffectFactory.GenerateCard("stinger", game, cardPrefab, this.gameObject), 1);
        AddCard(CardEffectFactory.GenerateCard("frontier runner", game, cardPrefab, this.gameObject), 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddCard(Card card, int position)
    {   
        card.location = Location.TRADE_ROW;
        cards.Add(card);

        card.gameObject.transform.Translate(new Vector3(position * Game.CARD_SIZE, 0, 0));
        card.gameObject.SetActive(true);
    }

    public void RemoveCard(Card card)
    {
        card.location = Location.UNDEFINED;
        card.gameObject.SetActive(false);
    }
}
