using UnityEditor;
using UnityEngine;

namespace Template
{
    public class CompositeSO : MEffectContainer, IEffectSO
    {
        public MEffectContainer container;

        public bool isManual;

        [TextArea]
        public string text;

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
            var effects = this.effects.ConvertAll(effect => ((IEffectSO)effect).CreateEffect(action)).ToArray();

            return new Effect.Composite(effects, text);
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