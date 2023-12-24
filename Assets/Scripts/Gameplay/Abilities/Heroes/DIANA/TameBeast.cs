using System.Text;
using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class TameBeast : ActiveAbility
    {
        [SerializeField] private Sprite freeIcon;
        [SerializeField] private int healthBonus;
        [SerializeField] private int defenseBonus;
        [SerializeField] private int attackPowerBonus;
        [SerializeField] private int speedBonus;

        public override string StatDescription =>
            CurrentCompanion is null
                ? new StringBuilder()
                    .Append(statDescription)
                    .Append($"<br>- Tamed beast gains +{healthBonus} health, +{defenseBonus} defense, +{attackPowerBonus} attack power and +{speedBonus} speed.")
                    .ToString()
                : "Hunter frees his companion, making it again agressive against him.";

        public override string Description => CurrentCompanion is null ? description : string.Empty;

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
               !Caster.Card.HasSpaceForHero()) return;
            
            Caster.Card.RemoveTokenWithoutUpdate(creature);
            Destroy(creature.gameObject);
            await Caster.Card.UpdateLayout(creature);

            CompanionToken companion = GlobalDefinitions.CreateCompanionToken(creature.Scriptable);
            companion.AddMaxHealth(healthBonus);
            companion.AddDefense(defenseBonus);
            companion.AddAttackPower(attackPowerBonus);
            companion.AddSpeed(speedBonus);
            Caster.Card.AddToken(companion, resetPosition: true, instantly: false);
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
            if(!CurrentCompanion.Card.HasSpaceForCreature()) return;
            
            CurrentCompanion.Card.RemoveTokenWithoutUpdate(CurrentCompanion);
            Destroy(CurrentCompanion.gameObject);
            await CurrentCompanion.Card.UpdateLayout(CurrentCompanion);
            
            CreatureToken creature = GlobalDefinitions.CreateCreatureToken(CurrentCompanion.Scriptable);
            CurrentCompanion.Card.AddToken(creature, resetPosition: true, instantly: false);
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
                  // creature.Scriptable.CreatureType is CreatureType.Beast && 
                  creature.Card == Caster.Card && 
                  Caster.Card.HasSpaceForHero()
                : ReferenceEquals(target, CurrentCompanion) && 
                  CurrentCompanion.Card.HasSpaceForCreature();
        }
    }
}