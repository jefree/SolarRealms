using Condition;

namespace Effect
{
    public class Conditional : Base
    {
        ICondition condition;
        Base[] effects;

        public Conditional(ICondition condition, params Base[] effects)
        {
            this.condition = condition;
            this.effects = effects;
        }

        public override bool Apply(Game game)
        {
            if (!condition.IsSatisfied(game))
            {
                return true;
            }

            foreach (var effect in effects)
            {
                effect.Apply(game);
            }

            return true;
        }

        public override string ID()
        {
            return $"CONDITIONAL";
        }
    }
}