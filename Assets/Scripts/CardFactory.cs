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

            default:
                card.cost = 1;
                break;
        }
    }

    public static Card GenerateCard(string name, Game game, GameObject cardPrefab, GameObject parent)
    {
        GameObject cardGameObject = Instantiate(cardPrefab, parent.transform);

        Card card = cardGameObject.GetComponent<Card>();
        card.cardName = name;
        card.game = game;

        Build(card, game);

        card.gameObject.SetActive(false);
        card.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{name}");

        return card;
    }

    public static void Viper(Card card, Game game)
    {
        BasicEffect effect = new(game, combat: 1);
        card.effects.Add(effect);
    }

    public static void Scout(Card card, Game game)
    {
        BasicEffect effect = new(game, trade: 1);
        effect.trade = 1;

        card.effects.Add(effect);
    }

    public static void BlobMiner(Card card, Game game)
    {
        card.cost = 2;
        card.effects.Add(new BasicEffect(game, trade: 3));
        card.effects.Add(new Effect.TradeRowScrap(game));
    }
}