using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Template
{
    public class ConditionalSO : MEffectContainer, IEffectSO
    {
        public MEffectContainer container;

        public bool isManual;

        public CardType conditionCardType;
        public int conditionCount;

        public void OnEnable()
        {
            effects.ForEach(effect => ((IEffectSO)effect).SetContainer(this));
        }

        public void Init(string name, MEffectContainer container)
        {
            this.name = name;
            this.container = container;
        }

        public void SetContainer(MEffectContainer container)
        {
            this.container = container;
        }

        public void Populate(Action action)
        {
            action.AddEffect(CreateEffect(action), isManual);
        }

        public virtual Effect.Base CreateEffect(Action action)
        {
            var condition = new Condition.TypeCard(conditionCardType, conditionCount);
            var effects = this.effects.ConvertAll(effect => ((IEffectSO)effect).CreateEffect(action)).ToArray();

            return new Effect.Conditional(condition, effects);
        }

#if UNITY_EDITOR
        [ContextMenu("Delete Effect")]
        private void Delete()
        {
            container.effects.Remove(this);

            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}