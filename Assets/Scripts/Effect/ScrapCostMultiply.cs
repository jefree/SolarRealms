using System.Linq;
using Mirror;
using UnityEngine;

namespace Effect
{
    public class ScrapCostMultiply : Manual, ICardReceiver, IConfirmNetable
    {

        public enum MultiplierType
        {
            Cost,
            Fixed
        }

        Basic effect;
        CardLocation location;
        Card card;
        MultiplierType type;
        bool force;

        public ScrapCostMultiply(Basic effect, CardLocation location, MultiplierType type, bool force = false)
        {
            this.effect = effect;
            this.location = location;
            this.type = type;
            this.force = force;
        }

        public override void ManualActivate(Game game)
        {
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this, !force);
        }

        public override bool Apply(Game game)
        {
            if (card != null)
            {
                var multiplier = type == MultiplierType.Cost ? card.cost : 1;
                var multEffect = new Basic(effect.combat * multiplier, effect.trade * multiplier, effect.authority * multiplier);

                multEffect.action = action;
                multEffect.Apply(game);
                game.ScrapCard(card);
            }

            return true;
        }

        public override string ID()
        {
            return $"SCRAP_X_COST {effect.ID()}";
        }

        public override string Text()
        {
            var suffix = type == MultiplierType.Cost ? " por su costo" : "";
            return $"Deshuesa una carta y gana {effect.Text()}{suffix}.";
        }

        public string ConfirmText()
        {

            var cardName = card != null ? card.cardName : "nada";

            return $"Deshuesar <b>{cardName}</b>?";
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (!location.HasFlag(card.location))
            {
                game.ShowLocalMessage("Ubicacion de carta no valida");
                return;
            }

            if (this.card != null)
            {
                this.card.isSelected = false;
            }

            this.card = card;
            this.card.isSelected = true;
        }

        public EffectState GetState()
        {
            return new EffectState(cards: new Card[] { card });
        }

        public void LoadState(EffectState state)
        {
            this.card = state.cards.First();
        }

        public override void CleanUp()
        {
            if (card != null)
            {
                card.isSelected = false;
            }
        }
    }
}