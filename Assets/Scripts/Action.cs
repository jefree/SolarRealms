using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class Action
{
    public bool fullyResolved = false;
    public string actionName = "";
    public Effect.Base currentEffect;
    Game game;
    public Card card;
    public List<Effect.Base> effects = new();
    protected List<Effect.Base> usedEffects = new();
    public List<Effect.Base> manualEffects = new();
    protected List<Effect.Base> usedManualEffects = new();
    List<Condition.ICondition> conditions = new();

    public Action(Card card, string name)
    {
        this.card = card;
        this.game = card.game;

        actionName = name;
    }

    public virtual void AddEffect(Effect.Base effect, bool isManual = false)
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

    [Server]
    protected void ActivateNextEffect()
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

    [Server]
    public virtual void OnEffectResolved(Effect.Base effect)
    {
        if (effect != currentEffect)
            throw new ArgumentException("Effect is not the current active");

        NetDisableEffect(effect);

        if (effect.isManual)
        {
            card.ActionResolved(this);
        }
        else
        {
            ActivateNextEffect();
        }

        if (effects.Count == 0 && manualEffects.Count == 0)
        {
            fullyResolved = true;

        }
    }

    [Server]
    public void OnEffectCanceled(Effect.Base effect)
    {
        if (effect != currentEffect)
            throw new ArgumentException("Effect is not the current active");

        // since only manual effects can be canceled we resolve this action right away
        card.ActionResolved(this);

    }

    public void NetDisableEffect(Effect.Base effect)
    {
        DisableEffect(effect);
        card.RpcDisableEffect(actionName, effect.ID(), effect.isManual);
    }

    public void DisableEffect(Effect.Base effect)
    {
        if (effect.isManual)
        {
            manualEffects.Remove(effect);
            usedManualEffects.Add(effect);
        }
        else
        {
            effects.Remove(effect);
            usedEffects.Add(effect);
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

    public virtual Effect.Base FindEffect(string id, bool isManual = false)
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