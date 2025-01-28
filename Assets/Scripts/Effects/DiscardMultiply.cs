using System.Collections.Generic;
using System.Linq;
using Mirror;

namespace Effect
{
    public class DiscardMultiply : Manual, ICardReceiver, IConfirmNetable
    {
        Basic basicEffect;
        List<Card> selectedCards = new();

        public DiscardMultiply(Effect.Basic effect)
        {
            basicEffect = effect;
        }

        public override void ManualActivate(Game game)
        {
            selectedCards.Clear();
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public override bool Apply(Game game)
        {
            var multiplier = selectedCards.Count;
            var effect = new Basic(basicEffect.combat * multiplier, basicEffect.trade * multiplier, basicEffect.authority * multiplier);

            effect.action = action;
            effect.Apply(game);

            selectedCards.ForEach(card => game.activePlayer.DiscardCard(card));

            return true;
        }

        public override string ID()
        {
            return $"DISCARD_X {basicEffect.ID()}";
        }

        public override string Text()
        {
            return $"Descarta cualquier numero de cartas y gana {basicEffect.Text()} por cada una";
        }

        public string ConfirmText()
        {
            return $"Descartar <b>{selectedCards.Count}</b> Carta(s) ?";
        }

        [Client]
        public void SetCard(Game game, Card card)
        {
            if (card.location != CardLocation.HAND)
            {
                game.ShowLocalMessage("Selecciona cartas de tu mano");
                return;
            }

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

        public override void CleanUp()
        {
            selectedCards.ForEach(card => card.isSelected = false);
        }
    }
}