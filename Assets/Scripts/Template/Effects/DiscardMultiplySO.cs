using System.Threading;

namespace Template
{
    public class DiscardMultiplySO : EffectSO
    {
        public int combat;
        public int trade;
        public int authority;

        public override Effect.Base CreateEffect(Action action)
        {
            var basic = new Effect.Basic(combat, trade, authority);
            return new Effect.DiscardMultiply(basic);
        }
    }
}