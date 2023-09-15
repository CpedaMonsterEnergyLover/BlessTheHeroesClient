namespace Effects
{
    public interface IEffectsPool
    {
        public bool IsForEffect<T>() where T : EffectObject;
        public EffectObject GetEffectObject();
        public void Pool(EffectObject effectObject);
    }
}