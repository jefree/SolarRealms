using GameStates;
using UnityEngine;

namespace ClickResolver
{
    public class Empty : Base
    {
        public override void ResolveClick(BasicState state, Card card)
        {
            Debug.Log("empty click");
        }
    }
}