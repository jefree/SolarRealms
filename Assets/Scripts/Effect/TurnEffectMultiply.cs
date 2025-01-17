using System.Linq;

namespace Effect
{
    public class TurnEffectMultiply : Base
    {
        string targetTurnEffect;
        Basic basic;

        public TurnEffectMultiply(string effect, int combat = 0, int trade = 0, int authority = 0)
        {

            basic = new(combat, trade, authority);
            targetTurnEffect = effect;
        }

        public override string ID()
        {
            return $"MULTIPLY turnEffect({targetTurnEffect}) {basic.ID()}";
        }

        public override bool Apply(Game game)
        {
            var multiplier = game.turnEffects.Count(effect => effect == targetTurnEffect) + 1;
            var effect = new Basic(basic.combat * multiplier, basic.trade * multiplier, basic.authority * multiplier);

            effect.action = action;
            effect.Apply(game);

            return true;
        }

        public override string Text()
        {
            return $"Gana {basic.Text()} por carta scrapeada este turno incluida esta";
        }
    }
}