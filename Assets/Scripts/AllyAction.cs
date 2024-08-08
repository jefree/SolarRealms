using System.Security.Cryptography;
using Condition;

public class AllyCardAction : Action
{
    public AllyCardAction(Game game, Card card) : base(game)
    {
        AddCondition(new AllyCardCondition(card));
    }
}