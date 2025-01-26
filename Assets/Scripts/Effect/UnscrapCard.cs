using UnityEngine;

namespace Effect
{
    public class UnscrapCard : Base
    {
        public Card card;

        public UnscrapCard(Card card = null)
        {
            this.card = card;
        }

        public override bool Apply(Game game)
        {
            if (card.location == CardLocation.SCRAP_HEAP)
            {
                card.gameObject.SetActive(true);
                game.activePlayer.FreeCard(card);
            }
            else
            {
                game.activePlayer.AddTurnEndEffect(this);
            }

            return true;
        }

        public override string ID()
        {
            return "UNSCRAP CARD";
        }
    }
}