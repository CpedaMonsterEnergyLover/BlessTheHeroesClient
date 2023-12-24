using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;

namespace Gameplay.Abilities
{
    public class HuntersCommand : ActiveAbility
    {
        private TameBeast tameAbility;
        
        public override async UniTask Cast(IInteractable target)
        {
            if(target is not CreatureToken creature) return;

            await Caster.Attack(creature);
            if (tameAbility is not null && 
                tameAbility.CurrentCompanion is not null &&
                tameAbility.CurrentCompanion.Card == creature.Card)
            {
                if(creature.Dead) return;
                await tameAbility.CurrentCompanion.Attack(creature);
            }
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        protected override void OnTokenSet(IToken token)
        {
            if (token is HeroToken hero && 
                hero.HasAbility("Tame Beast", out Ability ability) &&
                ability is TameBeast tameBeast)
            {
                tameAbility = tameBeast;
            }
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is CreatureToken {CanBeTargeted: true} creature &&
                   creature.IsInAttackRange(Caster) &&
                   Caster.AttackDiceAmount != 0;
        }
    }
}