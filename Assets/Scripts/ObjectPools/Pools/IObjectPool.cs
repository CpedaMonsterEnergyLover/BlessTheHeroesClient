namespace Effects
{
    public interface IObjectPool
    {
        public bool IsForEffect<T>() where T : PoolObject;
        public PoolObject GetEffectObject();
        public void Pool(PoolObject effectObject);
    }
}