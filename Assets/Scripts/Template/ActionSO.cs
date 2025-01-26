using System;
using UnityEditor;
using UnityEngine;

namespace Template
{
    public class ActionSO : MEffectContainer
    {
        public CardSO card;
        public Action.Type type;

        public void OnEnable()
        {
            effects.ForEach(effect => ((IEffectSO)effect).SetContainer(this));
        }

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

            effects.ForEach(effect => ((IEffectSO)effect).Populate(action));
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
        [ContextMenu("Delete Action")]
        private void Delete()
        {
            card.actions.Remove(this);
            Undo.DestroyObjectImmediate(this);

            while (effects.Count > 0)
            {
                var effect = effects[0];

                effects.Remove(effect);
                Undo.DestroyObjectImmediate((ScriptableObject)effect);
            }

            AssetDatabase.SaveAssets();
        }
#endif
    }
}
