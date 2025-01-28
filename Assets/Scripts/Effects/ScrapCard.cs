using System.Linq;
using Effect.Helper;
using Mirror;

namespace Effect
{
    public class ScrapCard : Manual, ICardReceiver, IConfirmNetable
    {
        Game game;
        CardSelector cards;
        CardLocation location;
        int count;

        public ScrapCard(CardLocation location, int count)
        {
            this.location = location;
            this.count = count;

            cards = new CardSelector(count, location);
        }

        public override void ManualActivate(Game game)
        {
            this.game = game;
            game.localPlayer.CmdStartChooseCard();
            game.StartConfirmEffect(this);
        }

        public override bool Apply(Game game)
        {
            cards.ForEach(card => game.ScrapCard(card));

            return true;
        }

        [Client]
        public void SetCard(Game game, Card card)
        {

            var result = cards.ProcessCard(card);

            if (result == Result.InvalidLocation)
            {
                game.ShowLocalMessage("carta no valida");
            }
        }

        public override string Text()
        {
            return $"Deshuesa {count} carta(s) de {location}";
        }

        public override string ID()
        {
            return $"SCRAP Location({location})";
        }

        public EffectState GetState()
        {
            var state = new EffectState(cards: cards.ToArray());

            return state;
        }

        public void LoadState(EffectState state)
        {
            cards.SetCards(state.cards);
        }

        public string ConfirmText()
        {
            return $"Deshuesar {cards.Count} carta(s)?";
        }

        public override void CleanUp()
        {
            cards.CleanUp();
        }
    }
}