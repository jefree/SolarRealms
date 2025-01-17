using System.Linq;
using Mirror;

namespace Effect
{
    public class ScrapCostMultiply : Manual, ICardReceiver, IConfirmNetable
    {
        Basic basicEffect;
        Card card;

        public ScrapCostMultiply(Basic effect)
        {
            basicEffect = effect;
        }

        public override void ManualActivate(Game game)
        {
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);

        }

        public override bool Apply(Game game)
        {
            if (card != null)
            {
                var multiplier = card.cost;
                var effect = new Basic(basicEffect.combat * multiplier, basicEffect.trade * multiplier, basicEffect.authority * multiplier);

                effect.action = action;
                effect.Apply(game);
                game.ScrapCard(card);
            }

            return true;
        }

        public override string ID()
        {
            return $"SCRAP_X_COST {basicEffect.ID()}";
        }

        public override string Text()
        {
            return $"Deshuesa una carta y gana {basicEffect.Text()} por su costo";
        }

        public string ConfirmText()
        {

            var cardName = card != null ? card.cardName : "nada";

            return $"Deshuesar <b>{cardName}</b>?";
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (card.location != CardLocation.HAND && card.location != CardLocation.DISCARD_PILE)
            {
                game.ShowLocalMessage("Carta invalida");
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