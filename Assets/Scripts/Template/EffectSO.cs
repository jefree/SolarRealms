using System;
using UnityEditor;
using UnityEngine;

namespace Template
{
    public class EffectSO : ScriptableObject, IEffectSO
    {
        public MEffectContainer container;
        public bool isManual;

        public virtual void Populate(Action action)
        {
            action.AddEffect(CreateEffect(action), isManual);
        }

        public virtual Effect.Base CreateEffect(Action action)
        {
            throw new NotImplementedException("EffectSO subclasses must implement CreateEffect");
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