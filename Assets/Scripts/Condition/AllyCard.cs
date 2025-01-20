namespace Condition
{
    public class AllyCard : ICondition
    {
        Card card;
        public AllyCard(Card card)
        {
            this.card = card;
        }

        public bool IsSatisfied(Game game)
        {
            var cards = game.activePlayer.playArea.FactionCards(card.faction);
            var result = cards.Contains(card) && cards.Count >= 2;

            return result;
        }
    }
}