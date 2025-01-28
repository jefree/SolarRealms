using UnityEditor.U2D.Aseprite;

namespace GameStates
{
    public abstract class BasicEvent
    {
        public abstract void Resolve(BasicState state);
    }

    public class PlayCardEvent : BasicEvent
    {
        Card card;

        public PlayCardEvent(Card card)
        {
            this.card = card;
        }

        public override void Resolve(BasicState state)
        {
            state.game.activePlayer.PlayCard(card);
            state.game.activePlayer.playArea.PendingCards().ForEach(card => ProcessCard(card, state));
        }

        void ProcessCard(Card card, BasicState state)
        {
            card.PendingActions().ForEach(action =>
           {
               action.effects.ForEach(effect => state.Enqueue(new EffectEvent(effect)));
           });
        }
    }

    public class EffectEvent : BasicEvent
    {
        Effect.Base effect;

        public EffectEvent(Effect.Base effect)
        {
            this.effect = effect;
        }

        public override void Resolve(BasicState state)
        {
            state.Transit(new ResolveEffectState(state, effect));
        }
    }

    public class AttackEnemyBaseEvent : BasicEvent
    {
        Card baseCard;

        public AttackEnemyBaseEvent(Card card)
        {
            baseCard = card;
        }

        public override void Resolve(BasicState state)
        {
            // state.game.localPlayer.CmdAttackBase(baseCard);
            state.game.AttackBase(baseCard);
        }
    }

    public class BuyCardEvent : BasicEvent
    {
        Card card;

        public BuyCardEvent(Card card)
        {
            this.card = card;
        }

        public override void Resolve(BasicState state)
        {
            state.game.AttackBase(card);
        }
    }
}