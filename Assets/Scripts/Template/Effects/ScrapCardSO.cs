namespace Template
{
    public class ScrapCardSO : EffectSO
    {
        public CardLocation location;
        public int count = 1;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.ScrapCard(location, count);
        }
    }
}