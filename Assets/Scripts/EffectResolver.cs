namespace EffectResolver
{
    public class ChooseCard
    {
        Location location;
        Effect.ICardReceiver effect;
        Game game;

        public ChooseCard(Game game, Location location)
        {
            this.game = game;
            this.location = location;
        }

        public void Start()
        {
            game.StartChooseCard();
        }

        public bool SetCard(Card card)
        {

            if (card.location == location)
            {
                effect.SetCard(card);
                effect.Resolve();

                return true;
            }

            return false;
        }
    }
}
