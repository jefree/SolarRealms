using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Action
{
    public bool fullyResolved = false;
    public string actionName = "";
    public Effect.Base currentEffect;
    Game game;
    public Card card;
    public List<Effect.Base> effects = new();
    List<Effect.Base> usedEffects = new();
    public List<Effect.Base> manualEffects = new();
    List<Effect.Base> usedManualEffects = new();
    List<Condition.ICondition> conditions = new();

    public Action(Game game, string name)
    {
        this.game = game;
        actionName = name;
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

        effect.action = this;
    }

    public void AddCondition(Condition.ICondition condition)
    {
        conditions.Add(condition);
    }

    public void Activate()
    {
        ActivateNextEffect();
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

    public bool HasPendingEffects(bool manual = false)
    {
        var remainingEffects = manual ? manualEffects.Count : effects.Count;

        return SatisfyConditions() && remainingEffects > 0;
    }

    void ActivateNextEffect()
    {
        if (effects.Count == 0)
        {
            currentEffect = null;

            card.ActionResolved(this);

            return;
        }

        currentEffect = effects[0];
        currentEffect.Activate(game);
    }

    public void EffectResolved(Effect.Base effect)
    {
        if (effect != currentEffect)
            throw new ArgumentException("Effect is not the current active");

        Debug.Log($"EFFECT IN ACTION: {actionName}");

        if (effect.isManual)
        {
            manualEffects.Remove(effect);
            usedManualEffects.Add(effect);

            game.CardResolved(card);
        }
        else
        {
            effects.Remove(effect);
            usedEffects.Add(effect);

            ActivateNextEffect();
        }

        if (effects.Count == 0 && manualEffects.Count == 0)
        {
            fullyResolved = true;
        }
    }

    public bool SatisfyConditions()
    {
        return conditions.All(condition => condition.IsSatisfied(game));
    }

    public void Reset()
    {
        fullyResolved = false;

        effects = effects.Concat(usedEffects).ToList();
        usedEffects.Clear();

        manualEffects = manualEffects.Concat(usedManualEffects).ToList();
        usedManualEffects.Clear();
    }

    public string Text()
    {
        return currentEffect.Text();
    }

    public Effect.Base FindEffect(string id, bool isManual = false)
    {
        var effectList = isManual ? manualEffects : effects;

        var effect = effectList.Find(effect => effect.ID() == id);

        if (effect == null)
        {
            throw new ArgumentException($"invalid effect id {id} for action {actionName} in card {card.cardName}");
        }

        return effect;
    }
}