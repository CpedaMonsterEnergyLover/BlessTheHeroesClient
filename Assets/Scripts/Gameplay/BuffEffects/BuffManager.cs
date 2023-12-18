using System.Collections.Generic;
using System.Linq;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace Gameplay.BuffEffects
{
    [RequireComponent(typeof(IToken))]
    public class BuffManager : MonoBehaviour
    {
        public IToken Token { get; private set; }

        private readonly Dictionary<Scriptable.BuffEffect, BuffEffect> effects = new();
        private Transform buffsTransform;

        public delegate void EffectEvent(IToken token, BuffEffect effect);
        public event EffectEvent OnEffectApplied;
        
        

        private void Awake()
        {
            Token = GetComponent<IToken>();
            buffsTransform = new GameObject("BuffEffects").transform;
            buffsTransform.SetParent(transform);
        }

        public IEnumerable<BuffEffect> ActiveBuffs => effects.Values.Where(e => e.isActiveAndEnabled && e.Scriptable.EffectDirection is BuffEffectDirection.Positive);
        public IEnumerable<BuffEffect> ActiveDebuffs => effects.Values.Where(e => e.isActiveAndEnabled && e.Scriptable.EffectDirection is BuffEffectDirection.Negative);

        public bool FindEffect(BuffEffect target, out BuffEffect found)
        {
            found = effects.Values.FirstOrDefault(b => b.Scriptable.Equals(target.Scriptable));
            return found is not null;
        }

        public BuffEffect[] FindEffectsOfType(BuffEffectType type)
        {
            return effects.Values.Where(e => e.Scriptable.EffectType == type).ToArray();
        }

        public void ApplyEffect(IEffectApplier applier, BuffEffect buff, int duration)
        {
            var scriptable = buff.Scriptable;
            if (effects.ContainsKey(scriptable))
            {
                var effect = effects[scriptable];
                effect.gameObject.SetActive(true);
                effect.Refresh(duration);
            }
            else
            {
                var effect = Instantiate(buff, buffsTransform);
                effect.Manager = this;
                effect.Applier = applier;
                effect.Refresh(duration);
                effects.Add(buff.Scriptable, effect);
            }

            OnEffectApplied?.Invoke(Token, buff);
        }
        
        public void RemoveEffect(Scriptable.BuffEffect buff)
        {
            if(!effects.ContainsKey(buff)) return;

            effects[buff].gameObject.SetActive(false);
        }
        
        public void RemoveExact(BuffEffect buff)
        {
            if(!effects.ContainsValue(buff)) return;

            effects[buff.Scriptable].gameObject.SetActive(false);
        }
    }
}