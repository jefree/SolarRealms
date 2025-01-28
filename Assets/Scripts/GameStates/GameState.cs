namespace GameStates
{
    public abstract class GameState
    {
        public Game game;
        public Handler handler;

        public GameState(Game game, Handler handler)
        {
            this.game = game;
            this.handler = handler;
        }

        public abstract void Update();
        public abstract void HandleClickCard(Card card);
        public abstract void HandleStartChooseCard();

        public void Transit(GameState state)
        {
            handler.ChangeState(state);
        }
    }
}