namespace Template
{
    public class BasicSO : EffectSO
    {
        public int combat;
        public int trade;
        public int authority;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.Basic(combat: combat, trade: trade, authority: authority);
        }
    }
}