using Mirror;
using UnityEngine;

namespace Effect
{
    public abstract class Base
    {
        public bool isManual;

        public Action action;

        public abstract string ID();
        public abstract bool Apply(Game game);

        [Client]
        public virtual void Dispatch(Game game)
        {
            game.localPlayer.CmdResolveEffect(ToNet());
        }

        [Server]
        public virtual void Activate(Game game)
        {
            Resolve(game);
        }

        public virtual void Resolve(Game game)
        {
            Apply(game);
            // action.OnEffectResolved(this);
        }

        public virtual void Animate(Card card)
        {
            // TODO: looks like this is broken at the moment
        }

        public virtual string Text() { return ""; }

        public NetEffect ToNet()
        {
            return new NetEffect(this);
        }
    }
}