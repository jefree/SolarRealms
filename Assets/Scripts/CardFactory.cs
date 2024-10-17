using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;



public class CardFactory : MonoBehaviour
{

    static Dictionary<string, Action<Card, Game>> cards = new();

    [Server]
    public static Card GenerateCard(string name, Game game, GameObject cardPrefab, GameObject parent, Player player = null)
    {
        GameObject cardGameObject = Instantiate(cardPrefab);
        Card card = cardGameObject.GetComponent<Card>();

        card.cardName = name;
        card.game = game;
        card.player = player;

        Build(card, game);

        NetworkServer.Spawn(cardGameObject);

        return card;
    }

    static void Init()
    {
        cards.Add("viper", Viper);
        cards.Add("scout", Scout);
        cards.Add("blob miner", BlobMiner);
        cards.Add("infested moon", InfestedMoon);
        cards.Add("integration port", IntegrationPort);
        cards.Add("hive queen", HiveQueen);
        cards.Add("enforcer mech", EnforcerMech);
        cards.Add("outland station", OutlandStation);
        cards.Add("reclamation station", ReclamationStation);
    }

    public static void Build(Card card, Game game)
    {
        if (cards.Count == 0)
            Init();

        var generator = cards[card.cardName];

        generator(card, game);
    }

    public static void Default(Card card, Game game)
    {
        card.cost = 1;
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic());
        card.mainAction.card = card;
    }

    public static void Viper(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(combat: 1));
        card.mainAction.card = card;
    }

    public static void Scout(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(trade: 1));
        card.mainAction.card = card;
    }

    public static void BlobMiner(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 2;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(trade: 3));
        card.mainAction.AddEffect(new Effect.ScrapCard(Location.TRADE_ROW), isManual: true);
        card.mainAction.card = card;

        card.scrapAction = new Action(game, "scrap");
        card.scrapAction.AddEffect(new Effect.Basic(combat: 2), isManual: true);
        card.scrapAction.card = card;
    }

    static void InfestedMoon(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.faction = Faction.THE_BLOBS;
        card.cost = 6;
        card.defense = 5;
        card.outpost = false;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(combat: 4));
        card.mainAction.card = card;

        card.allyAction = new AllyCardAction(game, card, "ally");
        card.allyAction.AddEffect(new Effect.DrawCard());
        card.allyAction.card = card;

        card.doubleAllyAction = new DoubleAllyCardAction(game, card, "doubleAlly");
        card.doubleAllyAction.AddEffect(new Effect.DrawCard());
        card.doubleAllyAction.card = card;
    }

    static void IntegrationPort(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 3;
        card.defense = 5;
        card.outpost = true;
        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(trade: 1));
        card.mainAction.card = card;
    }
    static void HiveQueen(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 7;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(combat: 7));
        card.mainAction.AddEffect(new Effect.DrawCard());
        card.mainAction.card = card;

        card.allyAction = new AllyCardAction(game, card, "ally");
        card.allyAction.AddEffect(new Effect.Basic(combat: 3));
        card.allyAction.card = card;

        card.doubleAllyAction = new DoubleAllyCardAction(game, card, "doubleAlly");
        card.doubleAllyAction.AddEffect(new Effect.Basic(combat: 3));
        card.doubleAllyAction.card = card;
    }

    static void EnforcerMech(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.cost = 5;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(combat: 5));
        card.mainAction.AddEffect(new Effect.ScrapCard(Location.HAND), isManual: true);
        card.mainAction.card = card;
    }

    static void OutlandStation(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.cost = 3;

        card.mainAction = new OrAction(game, "main");
        card.mainAction.AddEffect(new Effect.Basic(trade: 1), isManual: true);
        card.mainAction.AddEffect(new Effect.Basic(authority: 3), isManual: true);
        card.mainAction.card = card;

        card.scrapAction = new Action(game, "scrap");
        card.scrapAction.AddEffect(new Effect.DrawCard(), isManual: true);
        card.scrapAction.card = card;
    }

    static void ReclamationStation(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.cost = 3;

        card.mainAction = new Action(game, "main");
        card.mainAction.AddEffect(new Effect.ScrapCard(Location.DISCARD_PILE), isManual: true);
        card.mainAction.card = card;

        card.scrapAction = new Action(game, "scrap");
        card.scrapAction.AddEffect(new Effect.TurnEffectMultiply("scrap", combat: 3), isManual: true);
        card.scrapAction.card = card;
    }
}