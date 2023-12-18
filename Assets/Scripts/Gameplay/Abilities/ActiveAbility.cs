namespace Gameplay.Abilities
{
    public abstract class ActiveAbility : CastableAbility
    {
        public abstract void OnCastStart();
        public abstract void OnCastEnd();
    }
}