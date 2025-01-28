using System;
using System.Collections.Generic;

namespace GameStates
{
    public class BasicState : GameState
    {
        Queue<BasicEvent> events = new();

        public BasicState(Game game, Handler handler) : base(game, handler)
        {
        }

        public override void Update()
        {
            if (events.Count == 0) { return; }

            events.Dequeue().Resolve(this);
        }

        public override void HandleClickCard(Card card)
        {
            ClickResolver.Base resolver = CreateClickResolverFor(card);
            resolver.ResolveClick(this, card);
        }

        public override void HandleStartChooseCard()
        {
            throw new InvalidOperationException("Cannot start choose card on basic state");
        }

        public ClickResolver.Base CreateClickResolverFor(Card card)
        {
            if (card.player != game.localPlayer)
            {
                return card.location switch
                {
                    CardLocation.PLAY_AREA => new ClickResolver.EnemyPlayArea(),
                    _ => new ClickResolver.Empty()
                };
            }

            return card.location switch
            {
                CardLocation.HAND => new ClickResolver.Hand(),
                CardLocation.TRADE_ROW => new ClickResolver.Hand(),
                CardLocation.PLAY_AREA => new ClickResolver.PlayArea(),
                _ => new ClickResolver.Empty()
            };
        }

        public void Enqueue(BasicEvent evt)
        {
            events.Enqueue(evt);
        }



    }
}