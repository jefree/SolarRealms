using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayArea : MonoBehaviour
{

    List<Card> ships = new();
    List<Card> bases = new();

    Transform shipArea;
    Transform baseArea;
    Player player;

    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        shipArea = transform.Find("ShipArea");
        baseArea = transform.Find("BaseArea");
    }

    public void AddCard(Card card)
    {
        card.location = Location.PLAY_AREA;

        switch (card.type)
        {
            case CardType.SHIP:
                AddShip(card);
                break;
            case CardType.BASE:
                AddBase(card);
                break;
            default:
                throw new InvalidOperationException($"card type not support {card.type}");
        }
    }

    void AddShip(Card card)
    {
        card.transform.SetParent(shipArea);
        card.transform.localPosition = new Vector3(ships.Count * Game.CARD_SIZE, 0, 0);

        ships.Add(card);
    }

    void AddBase(Card card)
    {
        card.transform.SetParent(baseArea);
        card.transform.localPosition = new Vector3(bases.Count * Game.CARD_SIZE, 0, 0);
        card.transform.localRotation = new(0, 0, 0, 0);

        bases.Add(card);
    }

    public List<Card> DiscardShips()
    {
        var removedCards = new List<Card>();

        while (ships.Count > 0)
        {
            removedCards.Add(ships[0]);
            RemoveShip(ships[0]);
        }

        return removedCards;
    }

    public void ResetBases()
    {
        bases.ForEach(card => card.Reset());
    }

    public void ActivateBases()
    {
        bases.ForEach(card => player.game.PlayCard(card));
    }

    void RemoveShip(Card card)
    {
        card.transform.SetParent(null);
        card.location = Location.UNDEFINED;

        ships.Remove(card);
    }
    void RemoveBase(Card card)
    {
        card.transform.SetParent(null);
        card.location = Location.UNDEFINED;

        bases.Remove(card);
    }

    public void DestroyBase(Card card)
    {
        if (!bases.Contains(card))
        {
            throw new InvalidOperationException("Base do not belong to this player");
        }

        RemoveBase(card);
    }

    public bool HasOutpost()
    {
        return bases.Any(card => card.outpost);
    }
}
