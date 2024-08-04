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

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new BasicEffect());
    }

    public static void Viper(Card card, Game game)
    {
        BasicEffect effect = new(combat: 1);
        card.type = CardType.SHIP;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(effect);
    }

    public static void Scout(Card card, Game game)
    {
        BasicEffect effect = new(trade: 1);
        effect.trade = 1;

        card.type = CardType.SHIP;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(effect);
    }

    public static void BlobMiner(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.cost = 2;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new BasicEffect(trade: 3));

        card.scrapAction = new Action(game);
        card.scrapAction.AddEffect(new Effect.TradeRowScrap(), isManual: true);
    }

    static void InfestedMoon(Card card, Game game)
    {
        card.cost = 6;
        card.type = CardType.BASE;
        card.defense = 5;
        card.outpost = false;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new BasicEffect(combat: 3));
    }

    static void IntegrationPort(Card card, Game game)
    {
        card.cost = 3;
        card.type = CardType.BASE;
        card.defense = 5;
        card.outpost = true;
        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new BasicEffect(trade: 1));
    }
    static void HiveQueen(Card card, Game game)
    {
        card.type = CardType.SHIP;
        card.cost = 7;

        card.mainAction = new Action(game);
        card.mainAction.AddEffect(new BasicEffect(combat: 7));
    }
}