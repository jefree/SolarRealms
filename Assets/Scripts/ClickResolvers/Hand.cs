using GameStates;

namespace ClickResolver
{
    public class Hand : Base
    {
        public override void ResolveClick(BasicState state, Card card)
        {
            state.Enqueue(new PlayCardEvent(card));
        }
    }
}