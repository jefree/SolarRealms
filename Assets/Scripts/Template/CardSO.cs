using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Template
{
    [CreateAssetMenu(fileName = "card", menuName = "Card")]
    public class CardSO : ScriptableObject
    {
        public Sprite sprite;
        public CardType type;
        public Faction faction;
        public int cost;
        public int defense;
        public bool outpost;

        public List<ActionSO> actions = new();


        public void OnEnable()
        {
            actions.ForEach(action => action.card = this);
        }

        public void Populate(Card card)
        {
            card.image.sprite = sprite;
            card.cardName = card.name;
            card.type = type;
            card.faction = faction;
            card.cost = cost;
            card.defense = defense;
            card.outpost = outpost;

            actions.ForEach(action => action.Populate(card));
        }

#if UNITY_EDITOR
        [ContextMenu("New Main Action")]
        private void MakeMainAction()
        {
            MakeNewAction("main");
        }

        [ContextMenu("New Ally Action")]
        private void MakeAllyAction()
        {
            MakeNewAction("ally");
        }

        [ContextMenu("New Double Ally Action")]
        private void MakeDoubleAllyAction()
        {
            MakeNewAction("doubleAlly");
        }

        [ContextMenu("New Scrap Action")]
        private void MakeScrapAction()
        {
            MakeNewAction("scrap");
        }

        private void MakeNewAction(string type)
        {
            if (actions.Any(action => action.name == type))
            {
                throw new ArgumentException($"A Card cannot have multiple actions of name {type}");
            }

            ActionSO action = CreateInstance<ActionSO>();
            var actionType = type switch
            {
                "main" => Action.Type.Main,
                "ally" => Action.Type.AllyAction,
                "doubleAlly" => Action.Type.DoubleAllyAction,
                "scrap" => Action.Type.Main,
                _ => throw new NotImplementedException()
            };

            action.type = actionType;
            action.name = type;
            action.card = this;

            actions.Add(action);

            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(action);
        }
#endif
    }
}