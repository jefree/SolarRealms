using System;
using System.Collections.Generic;
using System.Linq;

public class Action
{
    public bool activated = false;
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
        if (!effects.Contains(effect) && !manualEffects.Contains(effect))
        {
            throw new ArgumentException("Effect does not belong to this card");
        }

        currentEffect = effect;
        currentEffect.Activate(game);
    }

    public bool HasPendingEffects()
    {
        return effects.Count > 0;
    }

    void ActivateNextEffect()
    {
        if (effects.Count == 0)
        {
            currentEffect = null;
            return;
        }

        currentEffect = effects[0];
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
        else
        {
            effects.Remove(effect);
            usedEffects.Add(effect);

            ActivateNextEffect();
        }

        if (effects.Count == 0 && manualEffects.Count == 0)
        {
            activated = true;
        }
    }

    public void Reset()
    {
        activated = false;

        effects = effects.Concat(usedEffects).ToList();
        usedEffects.Clear();

        manualEffects = manualEffects.Concat(usedManualEffects).ToList();
        usedManualEffects.Clear();
    }

    public string Text()
    {
        return currentEffect.Text();
    }
}