using GameStates;

namespace ClickResolver
{
    public class PlayArea : Base
    {
        public override void ResolveClick(BasicState state, Card card)
        {
            // This happens in local so need to be handle differntly ?
            // game.ShowEffectList(card);
        }
    }
}