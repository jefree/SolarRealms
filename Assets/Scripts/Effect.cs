using System.Reflection;
using Effect;
using Unity.VisualScripting;
using UnityEditor.PackageManager;



namespace Effect
{
    public abstract class Base
    {
        public bool isManual;

        public abstract void Activate(Game game);

        public abstract void Resolve(Game game);

        public virtual string Text() { return ""; }
    }

    public interface ICardReceiver
    {

        void SetCard(Card card);
    }

    public class Basic : Base
    {
        //Card card;
        public int combat;
        public int trade;
        public int authority;

        public Basic(int combat = 0, int trade = 0, int authority = 0)
        {
            this.combat = combat;
            this.trade = trade;
            this.authority = authority;
        }

        public override void Activate(Game game)
        {
            Resolve(game);
        }

        public override void Resolve(Game game)
        {
            Player activePlayer = game.activePlayer;

            activePlayer.combat += combat;
            activePlayer.trade += trade;
            activePlayer.authority += authority;

            game.EffectResolved(this);
        }
    }

    public class ScrapCard : Base, ICardReceiver
    {
        Game game;
        Card card;
        EffectResolver.ChooseCard resolver;
        Location location;


        public ScrapCard(Location location)
        {
            this.location = location;
        }

        public override void Activate(Game game)
        {
            this.game = game;

            if (resolver == null)
            {
                resolver = new(game, Location.TRADE_ROW);
            }

            resolver.Start();
        }

        public override void Resolve(Game game)
        {
            game.ScrapCard(card);
            game.EffectResolved(this);
        }

        public void SetCard(Card card)
        {
            if (card.location == location)
            {
                this.card = card;
                Resolve(game);
            }
            else
            {
                game.ShowMessage("carta no valida");
            }
        }

        public override string Text()
        {
            return $"deshuesa una carta de {location}";
        }
    }
}

