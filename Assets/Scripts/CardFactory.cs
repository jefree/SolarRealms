using System;
using System.Collections.Generic;
using Mirror;
using Template;
using UnityEngine;

public class CardFactory : MonoBehaviour
{

    static Dictionary<string, Action<Card>> cards = new();

    public static Card FromSO(string name, Game game, GameObject cardPrefab, GameObject parent, Player player = null)
    {
        GameObject cardGameObject = Instantiate(cardPrefab);
        Card card = cardGameObject.GetComponent<Card>();

        Populate(name, card);

        card.game = game;
        card.player = player;

        NetworkServer.Spawn(cardGameObject);

        return card;
    }
    public static void Populate(string name, Card card)
    {
        var template = Resources.Load<CardSO>($"Templates/{name}");
        card.name = name;
        template.Populate(card);
    }

    /*
    static void Init()
    {
        cards.Add("blob miner", BlobMiner);
        cards.Add("enforcer mech", EnforcerMech);
        cards.Add("frontier hawk", FrontierHawk);
        cards.Add("gateship", Gateship);
        cards.Add("hive queen", HiveQueen);
        cards.Add("infested moon", InfestedMoon);
        cards.Add("integration port", IntegrationPort);
        cards.Add("nanobot swarm", NanobotSwarm);
        cards.Add("neural nexus", NeuralNexus);
        cards.Add("orbital shuttle", OrbitalShuttle);
        cards.Add("outland station", OutlandStation);
        cards.Add("pulverizer", Pulverizer);
        cards.Add("reclamation station", ReclamationStation);
        cards.Add("repair mech", RepairMech);
        cards.Add("scout", Scout);
        cards.Add("viper", Viper);
        cards.Add("warpgate cruiser", WarpgateCruiser);
    }

    [Server]
    public static Card GenerateCard(string name, Game game, GameObject cardPrefab, GameObject parent, Player player = null)
    {
        GameObject cardGameObject = Instantiate(cardPrefab);
        Card card = cardGameObject.GetComponent<Card>();

        card.cardName = name;
        card.game = game;
        card.player = player;

        Build(card);

        NetworkServer.Spawn(cardGameObject);

        return card;
    }

    public static void Build(Card card)
    {
        if (cards.Count == 0)
            Init();


        card.Init();
        var generator = cards[card.cardName];

        generator(card);
    }

    public static void Viper(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.AddEffect(new Effect.Basic(combat: 1), "main");
    }

    public static void Scout(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.AddEffect(new Effect.Basic(trade: 1), "main");
    }

    public static void BlobMiner(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 2;

        card.AddEffect(new Effect.Basic(trade: 3), "main");
        card.AddEffect(new Effect.ScrapCard(CardLocation.TRADE_ROW, 1), "main", isManual: true);

        card.AddEffect(new Effect.Basic(combat: 2), "scrap", isManual: true);
    }

    static void InfestedMoon(Card card)
    {
        card.type = CardType.BASE;
        card.faction = Faction.THE_BLOBS;
        card.cost = 6;
        card.defense = 5;
        card.outpost = false;

        card.AddEffect(new Effect.Basic(combat: 4), "main");
        card.AddEffect(new Effect.DrawCard(1), "ally");
        card.AddEffect(new Effect.DrawCard(1), "doubleAlly");
    }

    static void IntegrationPort(Card card)
    {
        card.type = CardType.BASE;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 3;
        card.defense = 5;
        card.outpost = true;

        card.AddEffect(new Effect.Basic(trade: 1), "main");
    }
    static void HiveQueen(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 7;

        card.AddEffect(new Effect.Basic(combat: 7), "main");
        card.AddEffect(new Effect.DrawCard(1), "main");

        card.AddEffect(new Effect.Basic(combat: 3), "ally");
        card.AddEffect(new Effect.Basic(combat: 3), "doubleAlly");
    }


    static void EnforcerMech(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 5;

        card.AddEffect(new Effect.Basic(combat: 5), "main");
        card.AddEffect(new Effect.ScrapCard(CardLocation.HAND, 1), "main", isManual: true);

        card.AddEffect(new DrawCard(1), "scrap");
    }

    static void OutlandStation(Card card)
    {
        card.type = CardType.BASE;
        card.faction = Faction.TRADE_FEDERATION;
        card.cost = 3;

        card.mainAction = new OrAction(card, "main");
        card.AddEffect(new Effect.Basic(trade: 1), "main", isManual: true);
        card.AddEffect(new Effect.Basic(authority: 3), "main", isManual: true);

        card.AddEffect(new Effect.DrawCard(1), "scrap", isManual: true);
    }

    static void ReclamationStation(Card card)
    {
        card.type = CardType.BASE;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 6;

        card.AddEffect(new Effect.ScrapCard(CardLocation.DISCARD_PILE, 1), "main", isManual: true);
        card.AddEffect(new Effect.TurnEffectMultiply(TurnEffect.Scrap, combat: 3), "scrap", isManual: true);
    }

    static void WarpgateCruiser(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.STAR_EMPIRE;
        card.cost = 3;

        card.AddEffect(new Effect.DiscardMultiply(new Effect.Basic(combat: 2)), "main", isManual: true);
        card.AddEffect(new Effect.DrawCard(1), "ally");
    }

    static void Gateship(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.TRADE_FEDERATION;
        card.cost = 6;

        card.AddEffect(new Effect.AcquireCard(type: CardType.SHIP_BASE, maxCost: 6), "main", isManual: true);
        card.AddEffect(new Effect.Basic(authority: 5), "ally");
    }

    static void NeuralNexus(Card card)
    {
        card.type = CardType.BASE;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 7;

        card.outpost = true;
        card.defense = 6;

        card.AddEffect(new Effect.ScrapCostMultiply(
            new Effect.Basic(combat: 1),
            CardLocation.HAND_OR_DISCARD
        ), "main", isManual: true);

        card.AddEffect(new Effect.DrawCard(1), "ally");
    }

    static void FrontierHawk(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.STAR_EMPIRE;
        card.cost = 1;

        card.AddEffect(new Effect.Basic(combat: 3), "main");
        card.AddEffect(new Effect.DrawCard(1), "main");
        card.AddEffect(new Effect.ForceDiscard(1), "main");
    }

    static void RepairMech(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 4;

        card.mainAction = new OrAction(card, "main");
        card.AddEffect(new Basic(trade: 3), "main", isManual: true);
        card.AddEffect(new RecoverCard(CardLocation.DISCARD_PILE, CardType.BASE, 1), "main", isManual: true);

        card.AddEffect(new ScrapCard(CardLocation.HAND_OR_DISCARD, 1), "ally", isManual: true);
    }

    static void OrbitalShuttle(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.TRADE_FEDERATION;
        card.cost = 2;

        card.AddEffect(new Basic(trade: 3), "main");
        card.AddEffect(new Conditional(
            new Condition.TypeCard(CardType.BASE, 2),
            new Basic(authority: 4),
            new DrawCard(1)
        ), "main");
    }

    static void NanobotSwarm(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 8;

        card.AddEffect(new Basic(combat: 5), "main");
        card.AddEffect(new DrawCard(2), "main");
        card.AddEffect(new ScrapCard(CardLocation.HAND_OR_DISCARD, 2), "main", isManual: true);
    }

    static void Pulverizer(Card card)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 5;

        card.AddEffect(new ScrapCostMultiply(new Basic(combat: 1), CardLocation.TRADE_ROW, force: true), "main");
        card.AddEffect(new DrawCard(1), "ally");
    }
    */
}