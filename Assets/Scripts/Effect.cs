using System.Collections.Generic;
using System.Linq;
using Mirror;

namespace Effect
{
    public abstract class Base
    {
        public bool isManual;

        public Action action;

        public abstract string ID();
        public abstract void Apply(Game game);

        public virtual void Dispatch(Game game)
        {
            game.localPlayer.CmdResolveEffect(ToNet());
        }

        public virtual void Activate(Game game)
        {
            Resolve(game);
        }

        public virtual void Resolve(Game game)
        {
            Apply(game);
            action.OnEffectResolved(this);
        }

        public virtual void Animate(Card card)
        {
            // TODO: looks like this is broken at the moment
        }

        public virtual string Text() { return ""; }

        public NetEffect ToNet()
        {
            return new NetEffect(this);
        }
    }

    public abstract class Manual : Base
    {
        public abstract void ManualActivate(Game game);

        public override void Dispatch(Game game)
        {
            game.localPlayer.CmdSetCurrentEffect(ToNet());
            ManualActivate(game);
        }
    }

    public struct NetEffect
    {
        public Card card;
        public string actionName;
        public string id;
        public bool isManual;

        public NetEffect(Base effect)
        {
            card = effect.action.card;
            actionName = effect.action.actionName;
            id = effect.ID();
            isManual = effect.isManual;
        }

        public Effect.Base GetEffect()
        {
            var action = card.FindAction(actionName);
            return action.FindEffect(id, isManual: isManual);
        }
    }

    public struct EffectState
    {
        public Card[] cards;
    }

    public interface ICardReceiver
    {
        void SetCard(Game game, Card card);
    }

    public interface IConfirmable
    {
        void Confirm(Game game);
        void Cancel();

        string ConfirmText();
    }

    public interface INetable
    {
        NetEffect ToNet();
    }

    public interface IConfirmNetable : IConfirmable, INetable
    {
        public abstract EffectState GetState();
        public abstract void LoadState(EffectState state);
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

    public class ScrapCard : Manual, ICardReceiver, IConfirmNetable
    {
        Game game;
        Card card;
        Location location;

        public ScrapCard(Location location)
        {
            this.location = location;
        }

        public override void ManualActivate(Game game)
        {
            this.game = game;
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public override void Apply(Game game)
        {
            if (card == null) { return; }

            game.ScrapCard(card);
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (location.HasFlag(card.location))
            {
                this.card = card;
            }
            else
            {
                game.ShowLocalMessage("carta no valida");
            }
        }

        public override string Text()
        {
            return $"Deshuesa una carta de {location}";
        }

        public override string ID()
        {
            return $"SCRAP Location({location})";
        }

        public EffectState GetState()
        {
            var state = new EffectState();
            state.cards = new Card[] { card };

            return state;
        }

        public void LoadState(EffectState state)
        {
            this.card = state.cards.First();
        }

        public void Confirm(Game game)
        {
            Resolve(game);
        }

        public void Cancel()
        {
            action.OnEffectCanceled(this);
        }

        public string ConfirmText()
        {
            var cardName = card != null ? card.cardName : "Nada";
            return $"Deshuesar <b>{cardName}</b>?";
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
            return "DRAW";
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
            return $"MULTIPLY turnEffect({targetTurnEffect}) {basic.ID()}";
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

    public class DiscardMultiply : Manual, ICardReceiver, IConfirmNetable
    {
        int targetCount;
        Basic basicEffect;
        List<Card> selectedCards = new();

        public DiscardMultiply(int count, Effect.Basic effect)
        {
            targetCount = count;
            basicEffect = effect;
        }

        public override void ManualActivate(Game game)
        {
            selectedCards.Clear();
            game.localPlayer.CmdStartChooseCard();

            // Show confirm dialog for no limited selection effects, so effect can be apply at some point
            if (targetCount == int.MaxValue)
            {
                game.StartConfirmEffect(this);
            }
        }

        public override void Apply(Game game)
        {
            var multiplier = selectedCards.Count;
            var effect = new Basic(basicEffect.combat * multiplier, basicEffect.trade * multiplier, basicEffect.authority * multiplier);

            effect.action = action;
            effect.Apply(game);

            selectedCards.ForEach(card => game.activePlayer.DiscardCard(card));
        }

        public override string ID()
        {
            return $"DISCARD_X targetCount({targetCount}) {basicEffect.ID()}";
        }

        public override string Text()
        {
            return $"Descarta cualquier numero de cartas y gana {basicEffect.Text()} por cada una";
        }

        public string ConfirmText()
        {
            return $"Descartar <b>{selectedCards.Count}</b> Carta(s) ?";
        }

        public void Confirm(Game game)
        {
            Resolve(game);
        }

        public void Cancel()
        {
            action.OnEffectCanceled(this);
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (card.isSelected && selectedCards.Contains(card))
            {
                card.isSelected = false;
                selectedCards.Remove(card);
            }
            else
            {
                selectedCards.Add(card);
                card.isSelected = true;
            }
        }

        public EffectState GetState()
        {
            var state = new EffectState();
            state.cards = selectedCards.ToArray();

            return state;
        }

        public void LoadState(EffectState state)
        {
            selectedCards = state.cards.ToList();
        }
    }
}

