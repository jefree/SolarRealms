using System;
using UnityEditor;
using UnityEngine;

namespace Template
{

    public class EffectSO : ScriptableObject
    {
        public ActionSO action;
        public bool isManual;

        public virtual void Populate(Action action)
        {
            throw new NotImplementedException("EffectSO subclasses must implement this method");
        }

#if UNITY_EDITOR

        [ContextMenu("Delete Effect")]
        private void Delete()
        {
            action.effects.Remove(this);

            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}