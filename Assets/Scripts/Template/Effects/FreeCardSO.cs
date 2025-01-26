namespace Template
{
    public class FreeCardSO : EffectSO
    {
        public CardType type;
        public int maxCost;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.FreeCard(type, maxCost);
        }
    }
}