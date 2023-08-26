namespace Util.Interface
{
    public interface IInventoryItem
    {
        public int StackSize { get; }
        public string CategoryName { get; }
    }
}