namespace GameStates
{
    public class Handler
    {
        Game game;
        public GameState state;

        public Handler(Game game)
        {
            this.game = game;
            state = new BasicState(game, this);
        }

        public void Update()
        {
            state.Update();
        }

        public void ClickCard(Card card)
        {
            state.HandleClickCard(card);
        }

        public void ChooseCard()
        {
            state.HandleStartChooseCard();
        }

        public void ChangeState(GameState state)
        {
            this.state = state;
        }
    }

}