namespace Template
{
    public class DrawCardSO : EffectSO
    {
        public int count = 1;

        public override void Populate(Action action)
        {
            var effect = new Effect.DrawCard(count);
            action.AddEffect(effect, isManual);
        }
    }
}