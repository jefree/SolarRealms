using GameStates;

namespace ClickResolver
{
    public class TradeRow : Base
    {
        public override void ResolveClick(BasicState state, Card card)
        {
            state.Enqueue(new BuyCardEvent(card));
        }
    }
}