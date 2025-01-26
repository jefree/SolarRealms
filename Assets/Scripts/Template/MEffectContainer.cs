using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Template
{
    public class MEffectContainer : ScriptableObject
    {
        public List<ScriptableObject> effects = new();

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

        [ContextMenu("Add Effect/RecoverCard")]
        private void RecoverCard()
        {
            var effect = CreateInstance<RecoverCardSO>();
            MakeNewEffect(effect, "recover card");
        }

        [ContextMenu("Add Effect/Conditional")]
        private void Conditional()
        {
            var effect = CreateInstance<ConditionalSO>();
            MakeNewEffect(effect, "conditional");
        }

        [ContextMenu("Add Effect/FreeCard")]
        private void FreeCard()
        {
            var effect = CreateInstance<FreeCardSO>();
            MakeNewEffect(effect, "free card");
        }

        [ContextMenu("Add Effect/UnscrapCard")]
        private void UnscrapCard()
        {
            var effect = CreateInstance<UnscrapCardSO>();
            MakeNewEffect(effect, "unscrap card");
        }

        [ContextMenu("Add Effect/Composite")]
        private void Composite()
        {
            var effect = CreateInstance<CompositeSO>();
            MakeNewEffect(effect, "composite");
        }

        private void MakeNewEffect(IEffectSO effect, string type)
        {
            effect.Init($"{name}/{type}", this);
            effects.Add((ScriptableObject)effect);

            AssetDatabase.AddObjectToAsset((Object)effect, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty((Object)effect);
        }
#endif
    }
}
