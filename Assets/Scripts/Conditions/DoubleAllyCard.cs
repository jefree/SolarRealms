namespace Condition
{
    public class DoubleAllyCard : ICondition
    {
        Card card;
        public DoubleAllyCard(Card card)
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