using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
    public static void Build(Card card, Game game)
    {
        switch (card.cardName)
        {
            case "viper":
                Viper(card, game);
                break;

            case "scout":
                Scout(card, game);
                break;

            case "blob miner":
                BlobMiner(card, game);
                break;

            case "infested moon":
                InfestedMoon(card, game);
                break;

            case "integration port":
                IntegrationPort(card, game);
                break;

            case "hive queen":
                HiveQueen(card, game);
                break;

            case "enforcer mech":
                EnforcerMech(card, game);
                break;

            default:
                Default(card, game);
                break;
        }
    }

    public static Card GenerateCard(string name, Game game, GameObject cardPrefab, GameObject parent, Player player = null)
    {
        GameObject cardGameObject = Instantiate(cardPrefab, parent.transform);
        Card card = cardGameObject.GetComponent<Card>();

        card.cardName = name;
        card.game = game;
        card.player = player;

        Build(card, game);

        card.gameObject.SetActive(false);
        card.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{name}");

        return card;
    }

    public static void Default(Card card, Game game)
    {
        card.cost = 1;
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic());
    }

    public static void Viper(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;
        Effect.Basic effect = new(combat: 1);
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(effect);
    }

    public static void Scout(Card card, Game game)
    {
        Effect.Basic effect = new(trade: 1);
        effect.trade = 1;

        card.type = CardType.SHIP;
        card.faction = Faction.UNALIGNED;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(effect);
    }

    public static void BlobMiner(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 2;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic(trade: 3));

        card.scrapAction = new Action(game);
        card.scrapAction.AddEffect(new Effect.ScrapCard(Location.TRADE_ROW), isManual: true);
    }

    static void InfestedMoon(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.faction = Faction.THE_BLOBS;
        card.cost = 6;
        card.defense = 5;
        card.outpost = false;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic(combat: 4));

        card.allyAction = new AllyCardAction(game, card);
        card.allyAction.AddEffect(new Effect.DrawCard());

        card.doubleAllyAction = new DoubleAllyCardAction(game, card);
        card.doubleAllyAction.AddEffect(new Effect.DrawCard());
    }

    static void IntegrationPort(Card card, Game game)
    {
        card.type = CardType.BASE;
        card.faction = Faction.MACHINE_CULT;
        card.cost = 3;
        card.defense = 5;
        card.outpost = true;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic(trade: 1));
    }
    static void HiveQueen(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.faction = Faction.THE_BLOBS;
        card.cost = 7;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic(combat: 7));

        card.allyAction = new AllyCardAction(game, card);
        card.allyAction.AddEffect(new Effect.Basic(combat: 3));

        card.doubleAllyAction = new DoubleAllyCardAction(game, card);
        card.doubleAllyAction.AddEffect(new Effect.Basic(combat: 3));
    }

    static void EnforcerMech(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.cost = 5;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new Effect.Basic(combat: 5));
        card.mainAction.AddEffect(new Effect.ScrapCard(Location.HAND), isManual: true);
    }
}