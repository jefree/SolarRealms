namespace Template
{
    public class TurnEffectMultiplySO : EffectSO
    {
        public TurnEffect turnEffect;
        public int combat;
        public int trade;
        public int authority;

        public override void Populate(Action action)
        {
            var effect = new Effect.TurnEffectMultiply(turnEffect, combat, trade, authority);
            action.AddEffect(effect, isManual);
        }
    }
}