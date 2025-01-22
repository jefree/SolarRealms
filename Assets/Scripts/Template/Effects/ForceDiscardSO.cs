namespace Template
{
    public class ForceDiscardSO : EffectSO
    {
        public int count = 1;

        public override void Populate(Action action)
        {
            var effect = new Effect.ForceDiscard(count);
            action.AddEffect(effect, isManual);
        }
    }
}