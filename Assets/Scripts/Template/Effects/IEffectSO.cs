namespace Template
{
    public interface IEffectSO
    {
        public void Populate(Action action);
        public Effect.Base CreateEffect(Action action);
        public void Init(string name, MEffectContainer container);
        public void SetContainer(MEffectContainer container);
    }
}