namespace Template
{
    public class ScrapCardSO : EffectSO
    {
        public CardLocation location;
        public int count = 1;

        public override void Populate(Action action)
        {
            var effect = new Effect.ScrapCard(location, count);
            action.AddEffect(effect, isManual);
        }
    }
}