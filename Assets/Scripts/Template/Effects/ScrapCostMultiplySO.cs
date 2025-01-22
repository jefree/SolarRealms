namespace Template
{
    public class ScrapCostMultiplySO : EffectSO
    {
        public CardLocation location;
        public bool force;
        public int combat;
        public int trade;
        public int authority;

        public override void Populate(Action action)
        {
            var basic = new Effect.Basic(combat, trade, authority);
            var effect = new Effect.ScrapCostMultiply(basic, location, force);

            action.AddEffect(effect, isManual);
        }
    }
}