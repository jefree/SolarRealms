namespace Template
{
    public class RecoverCardSO : EffectSO
    {
        public CardLocation location;
        public CardType type;
        public int count = 1;

        public override Effect.Base CreateEffect(Action action)
        {
            return new Effect.RecoverCard(location, type, count);
        }
    }
}