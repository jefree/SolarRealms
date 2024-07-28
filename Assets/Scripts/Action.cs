using System;
using System.Collections.Generic;
using System.Linq;

public class Action
{
    public bool activated = true;
    public Effect.Base currentEffect;
    Game game;
    public List<Effect.Base> effects = new();
    List<Effect.Base> usedEffects = new();
    public List<Effect.Base> manualEffects = new();
    List<Effect.Base> usedManualEffects = new();

    public Action(Game game)
    {
        this.game = game;
    }

    public void Activate()
    {
        ActivateNextEffect();
    }

    public void AddEffect(Effect.Base effect, bool isManual = false)
    {
        if (!isManual)
        {
            effects.Add(effect);
        }
        else
        {
            manualEffects.Add(effect);
            effect.isManual = true;
        }
    }

    public void ActivateEffect(Effect.Base effect)
    {
        if (!effects.Contains(effect))
        {
            throw new ArgumentException("Effect does not belong to this card");
        }

        currentEffect = effect;
        currentEffect.Activate(game);
    }

    void ActivateNextEffect()
    {
        if (effects.Count == 0)
        {
            currentEffect = null;
            activated = true;


        }

        currentEffect = effects.Find(effect => !effect.isManual);
        currentEffect.Activate(game);
    }

    public void EffectResolved(Effect.Base effect)
    {
        if (effect != currentEffect)
        {
            throw new ArgumentException("Effect is not the current active");
        }

        if (effect.isManual)
        {
            manualEffects.Remove(effect);
            usedManualEffects.Add(effect);
        }

        effects.Remove(effect);
        usedEffects.Add(effect);

        ActivateNextEffect();
    }

    public void Reset()
    {
        activated = false;
        (effects, usedEffects) = (usedEffects, effects);
        manualEffects = usedManualEffects.Concat(manualEffects).ToList();
        usedManualEffects.Clear();
    }

    public string Text()
    {
        return currentEffect.Text();
    }
}