namespace Template
{
    public class UnscrapCardSO : EffectSO
    {
        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.UnscrapCard(action.card);
        }
    }
}