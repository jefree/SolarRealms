using System;
using System.Collections.Generic;
using Effect.Helper;

namespace Effect
{
    public class RecoverCard : Manual, ICardReceiver, IConfirmNetable
    {
        CardLocation location;
        CardType type;
        int count;

        CardSelector cards;

        public RecoverCard(CardLocation location, CardType type, int count = 1)
        {
            this.location = location;
            this.type = type;
            this.count = count;

            cards = new(count, location, type);
        }

        public override void ManualActivate(Game game)
        {
            game.activePlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public override bool Apply(Game game)
        {
            cards.ForEach(card =>
            {
                game.activePlayer.discardPile.RemoveCard(card);
                game.activePlayer.deck.Add(card, Deck.Location.TOP);
            });

            return true;
        }

        public override string ID()
        {
            return $"RECOVER location({location}) type({type}) count({count})";
        }

        public void SetCard(Game game, Card card)
        {
            var result = cards.ProcessCard(card);


            if (result == Result.InvalidType)
            {
                game.ShowLocalMessage("Tipo de carta invalida");
            }

            if (result == Result.InvalidLocation)
            {
                game.ShowLocalMessage("Ubication de carta invalida");
            }
        }

        public EffectState GetState()
        {
            return new EffectState(cards: cards.ToArray());
        }

        public void LoadState(EffectState state)
        {
            cards.SetCards(state.cards);
        }

        public string ConfirmText()
        {
            return $"Recuperar {cards.Count} carta(s)";
        }

        public override string Text()
        {
            return $"Pon {count} {type} de tu {location} en la cima de tu mazo";
        }
    }
}