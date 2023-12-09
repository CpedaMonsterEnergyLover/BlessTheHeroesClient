using UnityEngine;

namespace Util.Dice
{
    public abstract class BaseEvaluator : ScriptableObject
    {
        public abstract bool Evaluate(int roll, out int result);
        public abstract string Description { get; }
    }
}