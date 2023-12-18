using Scriptable;

namespace Gameplay.Tokens
{
    public interface IHeroToken : IToken
    {
        public delegate void EquipmentEvent(HeroToken hero, int slot, Equipment equipment);

        public event TokenEvent OnResurrect;
        
        public event EquipmentEvent OnEquipped;
        public event EquipmentEvent OnUnequipped;
    }
}