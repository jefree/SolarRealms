namespace Template
{
    public class ForceDiscardSO : EffectSO
    {
        public int count = 1;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.ForceDiscard(count);
        }
    }
}