using System.Collections.Generic;

public class StateHandler
{

    Game game;
    Queue<Event> events = new();

    public StateHandler(Game game)
    {
        this.game = game;
    }

    public void Update()
    {
        if (events.Count == 0) { return; }

        var next = events.Dequeue();

        next.effect.Activate(game);
        next.action.OnEffectResolved(next.effect);
    }

    public void ProcessCard(Card card)
    {
        var actions = card.PendingActions();

        actions.ForEach(action =>
        {
            var effects = action.effects;
            effects.ForEach(effect => Add(action, effect));
        });
    }

    public void Add(Action action, Effect.Base effect)
    {
        var evt = new Event(action, effect);
        events.Enqueue(evt);
    }
}

public class Event
{
    public Action action;
    public Effect.Base effect;

    public Event(Action action, Effect.Base effect)
    {
        this.action = action;
        this.effect = effect;
    }
}