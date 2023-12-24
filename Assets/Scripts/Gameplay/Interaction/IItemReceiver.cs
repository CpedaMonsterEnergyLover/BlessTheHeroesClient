using Gameplay.Inventory;

namespace Gameplay.Interaction
{
    public interface IItemReceiver
    {
        public InventoryManager InventoryManager { get; }
    }
}