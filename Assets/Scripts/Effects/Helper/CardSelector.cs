using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Effect.Helper
{
    public enum Result
    {
        Added,
        Removed,
        InvalidLocation,
        InvalidType
    }

    public class CardSelector
    {
        List<Card> cards = new();
        CardLocation validLocation;
        CardType validType;
        int maxCardsCount = 1;

        public int Count
        {
            get => cards.Count;
        }

        public CardSelector(int maxCards, CardLocation validLocation, CardType validType = CardType.SHIP_BASE)
        {
            this.maxCardsCount = maxCards;
            this.validLocation = validLocation;
            this.validType = validType;
        }

        public Result ProcessCard(Card card)
        {
            if (!validType.HasFlag(card.type))
            {
                return Result.InvalidType;
            }

            if (!validLocation.HasFlag(card.location))
            {
                return Result.InvalidLocation;
            }

            if (cards.Contains(card))
            {
                card.isSelected = false;
                cards.Remove(card);
                return Result.Removed;
            }

            card.isSelected = true;

            if (cards.Count < maxCardsCount)
            {
                cards.Add(card);
            }
            else
            {
                cards[0].isSelected = false;
                cards.RemoveAt(0);
                cards.Add(card);
            }

            return Result.Added;
        }

        public void CleanUp()
        {
            cards.ForEach(card => card.isSelected = false);
        }

        public void SetCards(Card[] cards)
        {
            this.cards = cards.ToList();
        }

        public Card First()
        {
            return cards.First();
        }

        public void ForEach(System.Action<Card> action)
        {
            cards.ForEach(action);
        }

        public Card[] ToArray()
        {
            return cards.ToArray();
        }
    }
}