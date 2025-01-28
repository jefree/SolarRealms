namespace Condition
{
    public class TypeCard : ICondition
    {
        CardType type;
        int count;

        public TypeCard(CardType type, int count)
        {
            this.type = type;
            this.count = count;
        }
        public bool IsSatisfied(Game game)
        {
            var cards = game.activePlayer.playArea.TypeCards(type);

            return cards.Count >= count;
        }
    }
}