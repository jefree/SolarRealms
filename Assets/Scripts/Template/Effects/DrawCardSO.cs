namespace Template
{
    public class DrawCardSO : EffectSO
    {
        public int count = 1;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.DrawCard(count);
        }
    }
}