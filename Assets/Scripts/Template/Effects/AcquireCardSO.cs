namespace Template
{
    public class AcquireCardSO : EffectSO
    {
        public CardType type;
        public int maxCost;

        public override void Populate(Action action)
        {
            var effect = new Effect.AcquireCard(type, maxCost);
            action.AddEffect(effect, isManual);
        }
    }
}