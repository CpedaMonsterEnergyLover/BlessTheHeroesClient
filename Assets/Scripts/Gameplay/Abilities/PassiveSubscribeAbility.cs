using Gameplay.Tokens;

namespace Gameplay.Abilities
{
    public abstract class PassiveSubscribeAbility : PassiveAbility
    {
        private bool subscribed;



        protected abstract void OnSubscribed(IToken token);
        protected abstract void OnUnSubscribed(IToken token);

        private bool TrySubscribe()
        {
            if(subscribed || Caster is null) return false;
            subscribed = true;
            return true;
        }

        private bool TryUnsubscribe()
        {
            if(!subscribed || Caster is null) return false;
            subscribed = false;
            return true;
        }
        
        protected override void OnTokenSet(IToken token)
        {
            base.OnTokenSet(token);
            if(TrySubscribe()) OnSubscribed(Caster);
        }

        private void OnEnable()
        {
            if(TrySubscribe()) OnSubscribed(Caster);
        }

        private void OnDisable()
        {
            if(TryUnsubscribe()) OnUnSubscribed(Caster);
        }
    }
}