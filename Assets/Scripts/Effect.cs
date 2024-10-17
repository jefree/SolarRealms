using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.Timeline;

namespace Effect
{
    public abstract class Base
    {
        public bool isManual;

        public Action action;

        public virtual void Activate(Game game)
        {
            Resolve(game);
        }

        public abstract void Apply(Game game);

        public virtual void Resolve(Game game)
        {
            Apply(game);
            action.EffectResolved(this);
        }

        public abstract string ID();

        public virtual void Animate(Card card)
        {

        }

        public virtual string Text() { return ""; }
    }

    public interface ICardReceiver
    {

        void SetCard(Card card);
    }

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

        public override void Apply(Game game)
        {
            Player activePlayer = game.activePlayer;

            activePlayer.combat += combat;
            activePlayer.trade += trade;
            activePlayer.authority += authority;
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
            return $"Basic C({combat}) A({authority}) T({trade})";
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

    public class ScrapCard : Base, ICardReceiver
    {
        Game game;
        Card card;
        EffectResolver.ChooseCard resolver;
        Location location;


        public ScrapCard(Location location)
        {
            this.location = location;
        }

        public override void Activate(Game game)
        {
            this.game = game;

            if (resolver == null)
            {
                resolver = new(game, Location.TRADE_ROW);
            }

            resolver.Start();
        }

        public override void Apply(Game game)
        {
            game.ScrapCard(card);
        }

        public void SetCard(Card card)
        {
            if (location.HasFlag(card.location))
            {
                this.card = card;
                Resolve(game);
            }
            else
            {
                game.ShowNetMessage("carta no valida");
            }
        }

        public override string Text()
        {
            return $"deshuesa una carta de {location}";
        }

        public override string ID()
        {
            return $"Scrap L({location})";
        }
    }

    public class DrawCard : Base
    {
        public override void Apply(Game game)
        {
            game.activePlayer.DrawCard();
        }

        public override string ID()
        {
            return "Draw";
        }

        public override string Text()
        {
            return "Roba un carta";
        }
    }

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
            return $"MULTIPLY E({targetTurnEffect}) {basic.ID()}";
        }

        public override void Apply(Game game)
        {
            var multiplier = game.turnEffects.Count(effect => effect == targetTurnEffect) + 1;
            var effect = new Basic(basic.combat * multiplier, basic.trade * multiplier, basic.authority * multiplier);

            effect.action = action;
            effect.Apply(game);
        }

        public override string Text()
        {
            return $"Gana {basic.Text()} por carta scrapeada este turno incluida esta";
        }
    }
}

