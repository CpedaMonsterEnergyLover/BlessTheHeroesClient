using System.Text;
using Gameplay.Tokens;
using Scriptable.AttackVariations;
using UI.Browsers;
using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : Equipment
    {
        [Header("Weapon data")]
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField] private AttackType attackType;
        [SerializeField] private BaseAttackVariation attackVariation;
        [SerializeField] private DamageType damageType;

        public override string CategoryName => $"{damageType.ColoredTitle} {AttackType} Weapon";
        public int AttackDiceAmount => attackDiceAmount;
        public DamageType DamageType => damageType;
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
            => TokenBrowser.SelectedToken is HeroToken {ActionPoints: > 0} hero &&
               hero.Scriptable.AttackType == AttackType;
    }
}