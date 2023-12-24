using System.Collections.Generic;
using Gameplay.Abilities;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Usable")]
    public class Usable : Item
    {
        [SerializeField] private ActiveAbility ability;
        
        public override bool AllowClick => TokenBrowser.SelectedToken is HeroToken {ActionPoints: > 0};
        public override int StackSize => 10;
        public override string CategoryName => "Usable";

        
        
        public override void Consume() { }

        public void Use(IInteractable target)
        {
            if(TokenBrowser.SelectedToken is not HeroToken hero) return;

            hero.SetActionPoints(hero.ActionPoints - 1);
            hero.InventoryManager.RemoveItem(this, 1);
            ability.SetToken(hero);
            ability.Cast(target);
        }

        public void GetUseTargets(List<IInteractable> targets)
        {
            if(TokenBrowser.SelectedToken is not HeroToken hero) return;
            
            ability.SetToken(hero);
            ability.GetTargetsList(targets);
        }
    }
}