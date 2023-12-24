namespace Pooling
{
    public interface IObjectPool
    {
        public bool IsForEffect<T>() where T : Poolable;
        public Poolable GetEffectObject();
        public void Pool(Poolable effectObject);
    }
}