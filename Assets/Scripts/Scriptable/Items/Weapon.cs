using System.Text;
using Gameplay.Inventory;
using Gameplay.Tokens;
using MyBox;
using Scriptable.AttackVariations;
using UI;
using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : Equipment
    {
        [Header("Weapon data")]
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField] private BaseAttackVariation attackVariation;
        [SerializeField] private AttackType attackType;

        public override string CategoryName => $"{AttackType} Weapon";
        public int AttackDiceAmount => attackDiceAmount;
        public virtual AttackType AttackType => attackType;
        public BaseAttackVariation AttackVariation => attackVariation;

        

        public override bool CanEquipInSlot(int slot) => slot == 0;
        public override int Slot => 0;

        public override StringBuilder GetStatsStringBuilder()
        {
            return new StringBuilder()
                .Append($"Allows you to throw {attackDiceAmount} dice{(attackDiceAmount > 1 ? "s" : string.Empty)} when performing a {AttackType} attack.\n")
                .Append(base.GetStatsStringBuilder());
        }
        
        public override bool AllowClick 
            => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0} hero &&
               hero.Scriptable.AttackType == AttackType;
    }
}