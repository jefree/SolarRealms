namespace Template
{
    public class TurnEffectMultiplySO : EffectSO
    {
        public TurnEffect turnEffect;
        public int combat;
        public int trade;
        public int authority;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.TurnEffectMultiply(turnEffect, combat, trade, authority);
        }
    }
}