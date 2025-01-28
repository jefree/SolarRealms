using GameStates;

namespace ClickResolver
{
    public class DiscardPile : Base
    {
        public override void ResolveClick(BasicState state, Card card)
        {
            // This happens locally
            // game.localPlayer.CmdPlayCard(card);
        }
    }
}