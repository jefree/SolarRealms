using System;
using System.Linq;

namespace Effect
{
    public class Composite : Base
    {
        Base[] effects;
        string text;

        public Composite(Base[] effects, string text)
        {
            if (effects.Any(effect => effect.isManual))
            {
                throw new ArgumentException("Composite effects can not contain manual effects.");
            }

            this.effects = effects;
            this.text = text;
        }

        public override bool Apply(Game game)
        {
            foreach (var effect in effects)
            {
                effect.Apply(game);
            }

            return true;
        }

        public override string ID()
        {
            return $"COMPOSITE";
        }

        public override string Text()
        {
            return text;
        }
    }
}