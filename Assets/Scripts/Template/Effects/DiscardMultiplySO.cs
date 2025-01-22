using System.Threading;

namespace Template
{
    public class DiscardMultiplySO : EffectSO
    {
        public int combat;
        public int trade;
        public int authority;

        public override void Populate(Action action)
        {
            var basic = new Effect.Basic(combat, trade, authority);
            var effect = new Effect.DiscardMultiply(basic);

            action.AddEffect(effect, isManual);
        }
    }
}