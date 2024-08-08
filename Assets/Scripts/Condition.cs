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

            var result = cards.Contains(card) && cards.Count >= 2;

            Debug.Log($"Ally Condition: {result}");

            return result;
        }
    }
}