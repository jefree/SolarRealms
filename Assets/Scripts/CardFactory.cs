using UnityEngine;

public class CardEffectFactory : MonoBehaviour {

    public static void Build(Card card) {
        switch(card.cardName) {
            case "viper": 
            Viper(card);
            break;

            case "scout": 
            Scout(card);
            break;

            default:
            break;
        }
    }

    public static Card GenerateCard(string name, Game game, GameObject cardPrefab, GameObject parent) {
        GameObject cardGameObject = Instantiate(cardPrefab, parent.transform);

        Card card = cardGameObject.GetComponent<Card>();
        card.cardName = name;
        card.game = game;

        Build(card);

        card.gameObject.SetActive(false);
        card.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{name}");

        return card;
    }

    public static void Viper(Card card) 
    {
        Effect effect = new();
        effect.card = card;

        effect.combat = 1;
        card.primaryMainEffect = effect;
    }

    public static void Scout(Card card) 
    {
        Effect effect = new();
        effect.card = card;
        effect.trade = 1;

        card.primaryMainEffect = effect;
    }    
}