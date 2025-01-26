namespace Template
{
    public class ScrapCostMultiplySO : EffectSO
    {
        public CardLocation location;
        public bool force;
        public Effect.ScrapCostMultiply.MultiplierType type;
        public int combat;
        public int trade;
        public int authority;

        public override Effect.Base CreateEffect(Action action)
        {
            var basic = new Effect.Basic(combat, trade, authority);
            return new Effect.ScrapCostMultiply(basic, location, type, force);
        }
    }
}