namespace Template
{
    public class BasicSO : EffectSO
    {
        public int combat;
        public int trade;
        public int authority;

        public override void Populate(Action action)
        {
            var effect = new Effect.Basic(combat: combat, trade: trade, authority: authority);
            action.AddEffect(effect, isManual);
        }
    }
}