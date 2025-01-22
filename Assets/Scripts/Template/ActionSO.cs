using System;
using System.Collections;
using System.Collections.Generic;
using Effect;
using UnityEditor;
using UnityEngine;

namespace Template
{
    public class ActionSO : ScriptableObject
    {
        public CardSO card;
        public Action.Type type;
        public List<EffectSO> effects = new();

        public void Populate(Card card)
        {
            Action action = name switch
            {
                "main" => card.mainAction = CreateAction(type, card, name),
                "ally" => card.allyAction = CreateAction(type, card, name),
                "doubleAlly" => card.doubleAllyAction = CreateAction(type, card, name),
                "scrap" => card.scrapAction = CreateAction(type, card, name),
                _ => throw new NotImplementedException()
            };

            effects.ForEach(effect => effect.Populate(action));
        }

        private Action CreateAction(Action.Type type, Card card, string name)
        {
            return type switch
            {
                Action.Type.Main => new Action(card, name),
                Action.Type.AllyAction => new AllyCardAction(card, name),
                Action.Type.DoubleAllyAction => new DoubleAllyCardAction(card, name),
                Action.Type.OrAction => new OrAction(card, name),
                _ => throw new NotImplementedException()
            };
        }

#if UNITY_EDITOR
        [ContextMenu("Add Effect/Basic")]
        private void Basic()
        {
            var effect = CreateInstance<BasicSO>();
            MakeNewEffect(effect, "basic");
        }

        [ContextMenu("Add Effect/ScrapCard")]
        private void ScrapCard()
        {
            var effect = CreateInstance<ScrapCardSO>();
            MakeNewEffect(effect, "scrap card");
        }

        [ContextMenu("Add Effect/DrawCard")]
        private void DrawCard()
        {
            var effect = CreateInstance<DrawCardSO>();
            MakeNewEffect(effect, "draw card");
        }

        [ContextMenu("Add Effect/TurnEffectM")]
        private void TurnEffectM()
        {
            var effect = CreateInstance<TurnEffectMultiplySO>();
            MakeNewEffect(effect, "turn effect m");
        }

        [ContextMenu("Add Effect/DiscardM")]
        private void DiscardM()
        {
            var effect = CreateInstance<DiscardMultiplySO>();
            MakeNewEffect(effect, "discard m");
        }

        [ContextMenu("Add Effect/AcquireCard")]
        private void AcquireCard()
        {
            var effect = CreateInstance<AcquireCardSO>();
            MakeNewEffect(effect, "acquire card");
        }

        [ContextMenu("Add Effect/ScrapCostM")]
        private void ScrapCostM()
        {
            var effect = CreateInstance<ScrapCostMultiplySO>();
            MakeNewEffect(effect, "scrap cost m");
        }

        [ContextMenu("Add Effect/ForceDiscard")]
        private void ForceDiscard()
        {
            var effect = CreateInstance<ForceDiscardSO>();
            MakeNewEffect(effect, "force discard");
        }

        private void MakeNewEffect(EffectSO effect, string type)
        {
            effect.name = $"{name}/{type}";
            effect.action = this;

            effects.Add(effect);

            AssetDatabase.AddObjectToAsset(effect, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(effect);
        }

        [ContextMenu("Delete Action")]
        private void Delete()
        {

            card.actions.Remove(this);
            Undo.DestroyObjectImmediate(this);

            while (effects.Count > 0)
            {
                var effect = effects[0];

                effects.Remove(effect);
                Undo.DestroyObjectImmediate(effect);
            }

            AssetDatabase.SaveAssets();
        }
#endif
    }
}
