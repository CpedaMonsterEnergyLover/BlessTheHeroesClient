using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Location/Anfilade")]
    public class Anfilade : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private List<Floor> floors = new();
        [SerializeField] private DiceSet monsterAttackDice;
        [SerializeField] private DiceSet monsterMagicDice;
        [SerializeField] private DiceSet monsterDefenseDice;

        public string Name => name;
        public DiceSet MonsterAttackDice => monsterAttackDice;
        public DiceSet MonsterMagicDice => monsterMagicDice;
        public DiceSet MonsterDefenseDice => monsterDefenseDice;

        

        public bool GetFloor(int index, out Floor floor)
        {
            floor = null;
            if (floors.Count <= index) return false;

            floor = floors[index];
            return true;
        }
    }
}