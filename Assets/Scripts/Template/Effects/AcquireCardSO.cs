namespace Template
{
    public class AcquireCardSO : EffectSO
    {
        public CardType type;
        public int maxCost;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.AcquireCard(type, maxCost);
        }
    }
}