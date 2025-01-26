namespace Effect
{
    public class FreeCard : AcquireCard
    {
        public FreeCard(CardType type, int maxCost) : base(type, maxCost)
        {
        }

        public override bool Apply(Game game)
        {
            if (card != null)
            {
                game.FreeCard(card);
            }

            return true;
        }

        public override string ID()
        {
            return $"FREE CARD type({type}) maxCost({maxCost})";
        }

        public override string Text()
        {
            return $"Consigue una carta de costo {maxCost} o menor gratis.";
        }
    }
}
