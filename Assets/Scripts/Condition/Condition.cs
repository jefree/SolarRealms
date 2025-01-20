using UnityEngine;

namespace Condition
{
    public interface ICondition
    {
        bool IsSatisfied(Game game);
    }
}