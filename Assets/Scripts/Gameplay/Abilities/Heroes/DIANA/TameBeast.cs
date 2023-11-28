using System.Text;
using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;
using Util.Enums;
using Util.Tokens;

namespace Gameplay.Abilities
{
    public class TameBeast : ActiveAbility
    {
        [SerializeField] private Sprite freeIcon;
        [SerializeField] private int healthBonus;
        [SerializeField] private int defenseBonus;
        [SerializeField] private int attackPowerBonus;
        [SerializeField] private int speedBonus;

        public override string DetailDescription =>
            CurrentCompanion is null
                ? new StringBuilder()
                    .Append(detailDescription)
                    .Append("<i><color=magenta><br>")
                    .Append(
                        $"- Tamed beast gains +{healthBonus} health, +{defenseBonus} defense, +{attackPowerBonus} attack power and +{speedBonus} speed.")
                    .Append("</i></color>")
                    .ToString()
                : "Hunter frees his companion, making it again agressive against him.";

        public override string LiteralDescription => CurrentCompanion is null ? literalDescription : string.Empty;

        public override Sprite Icon => CurrentCompanion is null ? icon : freeIcon;

        public override int Manacost => CurrentCompanion is null ? manacost : 0;

        public override string Title => CurrentCompanion is null ? title : "Free Tamed Beast";

        public override bool RequiresAct => CurrentCompanion is null;

        public CompanionToken CurrentCompanion { get; private set; }



        public override async UniTask Cast(IInteractable target)
        {
            if (CurrentCompanion is not null)
                await Free();
            else 
                await Tame(target);
        }

        private async UniTask Tame(IInteractable target)
        {
            if(target is not CreatureToken creature || 
               !Caster.TokenCard.HasSpaceForHero()) return;
            
            Caster.TokenCard.RemoveTokenWithoutUpdate(creature);
            Destroy(creature.gameObject);
            await Caster.TokenCard.UpdateLayout(creature);

            CompanionToken companion = GlobalDefinitions.CreateCompanionToken(creature.Scriptable);
            companion.AddMaxHealth(healthBonus);
            companion.AddDefense(defenseBonus);
            companion.AddAttackPower(attackPowerBonus);
            companion.AddSpeed(speedBonus);
            Caster.TokenCard.AddToken(companion, resetPosition: true, instantly: false);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            companion.SetHealth(Mathf.Clamp(creature.CurrentHealth + healthBonus, 1, companion.MaxHealth));

            CurrentCompanion = companion;
            if(AbilitySlot is not null)
            {
                AbilitySlot.UpdateResourceCost();
                AbilitySlot.UpdateIcon();
            }
        }

        private async UniTask Free()
        {
            if(!CurrentCompanion.TokenCard.HasSpaceForCreature()) return;
            
            CurrentCompanion.TokenCard.RemoveTokenWithoutUpdate(CurrentCompanion);
            Destroy(CurrentCompanion.gameObject);
            await CurrentCompanion.TokenCard.UpdateLayout(CurrentCompanion);
            
            CreatureToken creature = GlobalDefinitions.CreateCreatureToken(CurrentCompanion.Scriptable);
            CurrentCompanion.TokenCard.AddToken(creature, resetPosition: true, instantly: false);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            creature.SetHealth(Mathf.Clamp(CurrentCompanion.CurrentHealth - healthBonus, 1, int.MaxValue));

            CurrentCompanion = null;
            if(AbilitySlot is not null)
            {
                AbilitySlot.UpdateResourceCost();
                AbilitySlot.UpdateIcon();
            }
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return CurrentCompanion is null 
                ? target is CreatureToken {CanBeTargeted: true} creature && 
                  creature.Scriptable.CreatureType is CreatureType.Beast && 
                  creature.TokenCard == Caster.TokenCard && 
                  Caster.TokenCard.HasSpaceForHero()
                : ReferenceEquals(target, CurrentCompanion) && 
                  CurrentCompanion.TokenCard.HasSpaceForCreature();
        }
    }
}