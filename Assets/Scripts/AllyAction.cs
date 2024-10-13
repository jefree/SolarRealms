using Condition;

public class AllyCardAction : Action
{
    public AllyCardAction(Game game, Card card, string name) : base(game, name)
    {
        AddCondition(new AllyCardCondition(card));
    }
}

public class DoubleAllyCardAction : Action
{
    public DoubleAllyCardAction(Game game, Card card, string name) : base(game, name)
    {
        AddCondition(new DoubleAllyCardCondition(card));
    }
}