using Unity.VisualScripting;

namespace Effect
{
    public class DrawCard : Base
    {
        int targetCount = 1;

        public DrawCard(int count)
        {
            targetCount = count;
        }

        public override bool Apply(Game game)
        {
            for (int i = 0; i < targetCount; i++)
            {
                game.activePlayer.DrawCard();
            }


            return true;
        }

        public override string ID()
        {
            return $"DRAW targetCount({targetCount})";
        }

        public override string Text()
        {
            return $"Roba {targetCount}  carta(s)";
        }
    }
}