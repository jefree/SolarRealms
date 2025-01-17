using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Effect
{
    public class ForceDiscard : Manual, IConfirmNetable, ICardReceiver
    {
        List<Card> cards = new();
        int targetCount;

        public ForceDiscard(int count)
        {
            targetCount = count;
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
            Debug.Log($"set card {card.cardName}");

            if (card.location != CardLocation.HAND)
            {
                game.ShowLocalMessage("Selecciona cartas de tu mano");
            }

            // here we have an opportunity to abstract this logic of adding/removing
            // and mark as selected the cards.
            if (cards.Contains(card))
            {
                card.isSelected = false;
                cards.Remove(card);
                return;
            }

            card.isSelected = true;

            if (cards.Count < targetCount)
            {
                cards.Add(card);
            }
            else
            {
                cards[0].isSelected = false;
                cards.RemoveAt(0);
                cards.Add(card);
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
            cards = state.cards.ToList();
        }

        public override string ID()
        {
            return $"ForceDiscard targetCount({targetCount})";
        }
    }
}