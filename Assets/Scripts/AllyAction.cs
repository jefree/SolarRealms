using Condition;

public class AllyCardAction : Action
{
    public AllyCardAction(Card card, string name) : base(card, name)
    {
        AddCondition(new AllyCard(card));
    }

    public override string PrefixText()
    {
        return "Ally:";
    }
}

public class DoubleAllyCardAction : Action
{
    public DoubleAllyCardAction(Card card, string name) : base(card, name)
    {
        AddCondition(new DoubleAllyCard(card));
    }

    public override string PrefixText()
    {
        return "D-Ally:";
    }
}