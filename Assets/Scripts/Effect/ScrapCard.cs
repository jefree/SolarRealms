using System.Linq;
using Mirror;

namespace Effect
{
    public class ScrapCard : Manual, ICardReceiver, IConfirmNetable
    {
        Game game;
        Card card;
        CardLocation location;

        public ScrapCard(CardLocation location)
        {
            this.location = location;
        }

        public override void ManualActivate(Game game)
        {
            this.game = game;
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public override bool Apply(Game game)
        {
            if (card != null)
            {
                game.ScrapCard(card);
            }

            return true;
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (card.location.HasFlag(location))
            {

                if (this.card != null)
                {
                    this.card.isSelected = false;
                }

                this.card = card;
                this.card.isSelected = true;
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

        public string ConfirmText()
        {
            var cardName = card != null ? card.cardName : "Nada";
            return $"Deshuesar <b>{cardName}</b>?";
        }

        public override void CleanUp()
        {
            if (this.card != null)
            {
                this.card.isSelected = false;
            }
        }
    }
}