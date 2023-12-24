using Gameplay.Abilities;
using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class Consumable : Item
    {
        [SerializeField] private InstantAbility ability;

        
        public override bool AllowClick => TokenBrowser.SelectedToken is HeroToken {ActionPoints: > 0};
        public override int StackSize => 10;
        public override string CategoryName => "Consumable";

        
        
        public override void Consume()
        {
            if(TokenBrowser.SelectedToken is not HeroToken hero) return;

            hero.SetActionPoints(hero.ActionPoints - 1);
            hero.InventoryManager.RemoveItem(this, 1);
            ability.SetToken(hero);
            ability.Cast(hero);
        }
    }
}