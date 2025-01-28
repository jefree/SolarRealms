using GameStates;

namespace ClickResolver
{
    public class EnemyPlayArea : Base
    {

        public override void ResolveClick(BasicState state, Card card)
        {
            if (card.type == CardType.BASE)
            {
                state.Enqueue(new AttackEnemyBaseEvent(card));
            }
        }
    }
}