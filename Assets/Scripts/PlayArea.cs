using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayArea : NetworkBehaviour
{

    readonly SyncListCard ships = new();
    readonly SyncListCard bases = new();

    Transform shipArea;
    Transform baseArea;
    Player player;

    public override void OnStartClient()
    {
        ships.Callback += OnUpdateShips;
        bases.Callback += OnUpdateBases;
    }

    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        shipArea = transform.Find("ShipArea");
        baseArea = transform.Find("BaseArea");
    }

    void OnUpdateShips(SyncListCard.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_ADD:
                OnShipAdded(newCard);
                break;
        }
    }

    void OnUpdateBases(SyncListCard.Operation op, int index, Card oldCard, Card newCard)
    {
        switch (op)
        {
            case SyncListCard.Operation.OP_ADD:
                OnBaseAdded(newCard);
                break;
        }
    }

    public void AddCard(Card card)
    {
        card.location = Location.PLAY_AREA;

        switch (card.type)
        {
            case CardType.SHIP:
                ships.Add(card);
                break;
            case CardType.BASE:
                bases.Add(card);
                break;
            default:
                throw new InvalidOperationException($"card type not support {card.type}");
        }
    }

    [Client]
    void OnShipAdded(Card card)
    {
        card.transform.SetParent(shipArea);
        card.transform.localPosition = new Vector3((ships.Count - 1) * Game.CARD_SIZE, 0, 0);

    }

    [Client]
    void OnBaseAdded(Card card)
    {
        card.transform.SetParent(baseArea);
        card.transform.localPosition = new Vector3((bases.Count - 1) * Game.CARD_SIZE, 0, 0);
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
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
        foreach (var card in bases)
        {
            card.Reset();
        }
    }

    public void ActivateBases()
    {
        foreach (var card in bases)
        {
            player.game.PlayCard(card);
        }
    }

    void RemoveShip(Card card)
    {
        card.transform.SetParent(null);
        card.location = Location.UNDEFINED;

        ships.Remove(card);
    }
    void RemoveBase(Card card)
    {
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

    public List<Card> FactionCards(Faction faction)
    {
        List<Card> factionCards = new();

        factionCards.AddRange(ships.FindAll(card => card.faction == faction));
        factionCards.AddRange(bases.FindAll(card => card.faction == faction));

        return factionCards;
    }

    public List<Card> PendingCards()
    {
        List<Card> pendingCards = new();

        pendingCards.AddRange(ships.FindAll(card => card.HasPendingActions()));
        pendingCards.AddRange(bases.FindAll(card => card.HasPendingActions()));

        return pendingCards;
    }

}
