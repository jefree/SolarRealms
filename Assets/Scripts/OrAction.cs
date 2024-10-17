using System;
using Mirror;

public class OrAction : Action
{
    public OrAction(Game game, string name) : base(game, name)
    {

    }

    public override void AddEffect(Effect.Base effect, bool isManual = true)
    {
        if (!isManual)
            throw new ArgumentException("Or Actions can only have manual actions");

        manualEffects.Add(effect);
        effect.isManual = true;
        effect.action = this;
    }

    [Server]
    //God forgive for what i've done here
    public override void EffectResolved(Effect.Base effect)
    {
        if (effect != currentEffect)
            throw new ArgumentException("Effect is not the current active");

        while (manualEffects.Count > 0)
        {
            NetDisableEffect(manualEffects[0]);
        }

        card.ActionResolved(this);
        fullyResolved = true;
    }

    public override Effect.Base FindEffect(string id, bool isManual = true)
    {
        if (!isManual)
            throw new ArgumentException("Or Actions can only have manual actions");

        var effect = manualEffects.Find(effect => effect.ID() == id);

        if (effect == null)
        {
            throw new ArgumentException($"invalid effect id {id} for action {actionName} in card {card.cardName}");
        }

        return effect;
    }
}