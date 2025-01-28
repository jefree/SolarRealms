using Mirror;

namespace Effect
{
    public abstract class Manual : Base
    {
        public abstract void ManualActivate(Game game);

        [Server]
        public override void Activate(Game game)
        {
            var conn = game.activePlayer.netIdentity.connectionToClient;
            game.TargetStartConfirm(conn, ToNet());
        }

        [Client]
        public override void Dispatch(Game game)
        {
            game.localPlayer.CmdSetCurrentEffect(ToNet());
            ManualActivate(game);
        }

        public override void Resolve(Game game)
        {
            if (Apply(game))
            {
                game.CloseNetConfirm();
                action.OnEffectResolved(this);
            }
        }

        public void Confirm(Game game)
        {
            Resolve(game);
        }

        public void Cancel()
        {
            action.OnEffectCanceled(this);
        }

        public virtual void CleanUp()
        {

        }
    }
}