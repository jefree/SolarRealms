using Condition;

public class AllyCardAction : Action
{
    public AllyCardAction(Card card, string name) : base(card, name)
    {
        AddCondition(new AllyCardCondition(card));
    }
}

public class DoubleAllyCardAction : Action
{
    public DoubleAllyCardAction(Card card, string name) : base(card, name)
    {
        AddCondition(new DoubleAllyCardCondition(card));
    }
}