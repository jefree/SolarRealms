using UnityEngine;

namespace Condition
{
    public interface ICondition
    {
        bool IsSatisfied(Game game);
    }

    public class AllyCardCondition : ICondition
    {
        Card card;
        public AllyCardCondition(Card card)
        {
            this.card = card;
        }

        public bool IsSatisfied(Game game)
        {
            var cards = game.activePlayer.playArea.FactionCards(card.faction);
            cards.ForEach(card => Debug.Log($"ALLY {card.cardName}"));
            var result = cards.Contains(card) && cards.Count >= 2;

            return result;
        }
    }

    public class DoubleAllyCardCondition : ICondition
    {
        Card card;
        public DoubleAllyCardCondition(Card card)
        {
            this.card = card;
        }

        public bool IsSatisfied(Game game)
        {
            var cards = game.activePlayer.playArea.FactionCards(card.faction);
            var result = cards.Contains(card) && cards.Count >= 3;

            return result;
        }
    }
}