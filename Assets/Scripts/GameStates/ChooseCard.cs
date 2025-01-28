using Effect;

namespace GameStates
{
    public class ChooseCardState : GameState
    {
        public ChooseCardState(GameState parent, ICardReceiver cardReceiver) : base(parent.game, parent.handler)
        {
        }

        public override void HandleClickCard(Card card)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleStartChooseCard()
        {
            game.StartChooseCard();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}