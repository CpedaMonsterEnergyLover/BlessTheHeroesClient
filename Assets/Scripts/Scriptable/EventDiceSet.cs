using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Dice Sets/Event")]
    public class EventDiceSet : DiceSet
    {
        public override int DiceAmount => 1;
    }
}