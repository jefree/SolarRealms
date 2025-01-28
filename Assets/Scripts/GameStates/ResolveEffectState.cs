using Effect;

namespace GameStates
{
    public class ResolveEffectState : GameState
    {
        Effect.Base effect;

        public ResolveEffectState(GameState parent, Effect.Base effect) : base(parent.game, parent.handler)
        {
            this.effect = effect;
        }

        public override void HandleClickCard(Card card)
        {

        }

        public override void HandleStartChooseCard()
        {
            Transit(new ChooseCardState(this, (ICardReceiver)effect));
        }

        public override void Update()
        {
            effect.Activate(game);
        }
    }
}