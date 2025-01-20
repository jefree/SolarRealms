using System.Collections.Generic;
using System.Linq;
using Effect.Helper;
using UnityEngine;

namespace Effect
{
    public class ForceDiscard : Manual, IConfirmNetable, ICardReceiver
    {
        CardSelector cards;
        int targetCount;

        public ForceDiscard(int count)
        {
            targetCount = count;
            cards = new(count, CardLocation.HAND);
        }

        public override void ManualActivate(Game game)
        {
            game.activePlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this, showCancel: false);
        }

        public override bool Apply(Game game)
        {

            if (cards.Count != targetCount)
            {
                game.ShowNetMessage($"selecciona {targetCount} cartas");
                return false;
            }

            cards.ForEach(card => game.activePlayer.DiscardCard(card));

            return true;
        }

        public void SetCard(Game game, Card card)
        {
            var result = cards.ProcessCard(card);

            if (result == Result.InvalidLocation)
            {
                game.ShowLocalMessage("Selecciona cartas de tu mano");
            }
        }

        public string ConfirmText()
        {
            if (cards.Count == 1)
            {
                return $"Descartar {cards.First().cardName}?";
            }
            else if (cards.Count > 1)
            {
                return $"Descartar {cards.Count} carta(s)?";
            }
            else
            {
                return $"Selecciona {targetCount} carta(s) de la mano para descartar";
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

        public override string ID()
        {
            return $"ForceDiscard targetCount({targetCount})";
        }
    }
}