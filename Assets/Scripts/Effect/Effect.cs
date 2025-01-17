using System;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace Effect
{
    public struct NetEffect
    {
        public Card card;
        public string actionName;
        public string id;
        public bool isManual;

        public NetEffect(Base effect)
        {
            card = effect.action.card;
            actionName = effect.action.actionName;
            id = effect.ID();
            isManual = effect.isManual;
        }

        public Effect.Base GetEffect()
        {
            var action = card.FindAction(actionName);
            return action.FindEffect(id, isManual: isManual);
        }
    }

    public struct EffectState
    {
        public Card[] cards;

        public EffectState(Card[] cards)
        {
            this.cards = cards;
        }
    }

    public interface ICardReceiver
    {
        void SetCard(Game game, Card card);
    }

    public interface IConfirmable
    {
        void Confirm(Game game);
        void Cancel();
        void CleanUp();
        string ConfirmText();
    }

    public interface INetable
    {
        NetEffect ToNet();
    }

    public interface IConfirmNetable : IConfirmable, INetable
    {
        public abstract EffectState GetState();
        public abstract void LoadState(EffectState state);
    }
}