using System.Linq;

namespace Effect
{
    public class AcquireCard : Manual, ICardReceiver, IConfirmNetable
    {
        protected Card card;
        protected CardType type;
        protected int maxCost;

        public AcquireCard(CardType type, int maxCost)
        {
            this.type = type;
            this.maxCost = maxCost;
        }

        public override bool Apply(Game game)
        {
            if (card != null)
            {
                game.AcquireCard(card, Deck.Location.TOP);
            }

            return true;
        }

        public string ConfirmText()
        {
            var cardName = card != null ? card.cardName : "nada";

            return $"Adquirir <b>{cardName}</b>?";
        }

        public EffectState GetState()
        {
            return new EffectState(cards: new Card[] { card });
        }

        public override string ID()
        {
            return $"ACQUIRE CARD type({type}) maxCost({maxCost})";
        }

        public void LoadState(EffectState state)
        {
            this.card = state.cards.First();
        }

        public override void ManualActivate(Game game)
        {
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public void SetCard(Game game, Card card)
        {
            if (card.location != CardLocation.TRADE_ROW)
            {
                game.ShowLocalMessage("Selecciona una carta del mercado");
                return;
            }

            if (card.cost > maxCost)
            {
                game.ShowLocalMessage("Carta con un costo mayor al permitido");
                return;
            }

            if (this.card != null)
            {
                this.card.isSelected = false;
            }

            this.card = card;
            this.card.isSelected = true;
        }

        public override string Text()
        {
            return $"Agrega una nave o base de costo {maxCost} o menor a la cima de tu baraja";
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