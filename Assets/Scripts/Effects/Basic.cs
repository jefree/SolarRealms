using System.Collections.Generic;

namespace Effect
{
    public class Basic : Base
    {
        //Card card;
        public int combat;
        public int trade;
        public int authority;

        public Basic(int combat = 0, int trade = 0, int authority = 0)
        {
            this.combat = combat;
            this.trade = trade;
            this.authority = authority;
        }

        public override bool Apply(Game game)
        {
            Player activePlayer = game.activePlayer;

            activePlayer.combat += combat;
            activePlayer.trade += trade;
            activePlayer.authority += authority;

            return true;
        }
        public override void Animate(Card card)
        {
            if (combat > 0)
            {
                card.RpcShowEffect(EffectColor.COMBAT, combat);
            }

            if (authority > 0)
            {
                card.RpcShowEffect(EffectColor.AUTHORITY, authority);
            }

            if (trade > 0)
            {
                card.RpcShowEffect(EffectColor.TRADE, trade);
            }
        }

        public override string ID()
        {
            return $"BASIC C({combat}) A({authority}) T({trade})";
        }

        public override string Text()
        {

            List<string> effects = new();

            if (combat > 0)
            {
                effects.Add($"Combate +{combat}");
            }

            if (authority > 0)
            {
                effects.Add($"Authoridad +{authority}");
            }

            if (trade > 0)
            {
                effects.Add($"Comercio +{trade}");
            }

            return string.Join(",", effects);
        }
    }
}