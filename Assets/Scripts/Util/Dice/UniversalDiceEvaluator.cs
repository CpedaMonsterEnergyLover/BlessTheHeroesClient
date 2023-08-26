namespace Util.Dice
{
    [System.Serializable]
    public abstract class UniversalDiceEvaluator
    {
        public abstract bool Evaluate(int roll, out int result);
        public abstract string Description { get; }
    }
}