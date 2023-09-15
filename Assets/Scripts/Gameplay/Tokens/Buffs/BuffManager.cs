using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Tokens.Buffs
{
    [RequireComponent(typeof(IToken))]
    public class BuffManager : MonoBehaviour
    {
        public IToken Token { get; private set; }

        private readonly Dictionary<Scriptable.BuffEffect, BuffEffect> effects = new();
        private Transform buffsTransform;
        

        private void Awake()
        {
            Token = GetComponent<IToken>();
            buffsTransform = new GameObject("BuffEffects").transform;
            buffsTransform.SetParent(transform);
        }

        public IEnumerable<BuffEffect> ActiveBuffs => effects.Values.Where(e => e.Scriptable.EffectType is BuffEffectType.Positive && e.enabled);
        public IEnumerable<BuffEffect> ActiveDebuffs => effects.Values.Where(e => e.Scriptable.EffectType is BuffEffectType.Negative && e.enabled);

        public void ApplyEffect(BuffEffect buff, int duration)
        {
            var scriptable = buff.Scriptable;
            if (effects.ContainsKey(scriptable))
            {
                var effect = effects[scriptable];
                effect.enabled = true;
                effect.Refresh(duration);
            }
            else
            {
                var effect = Instantiate(buff, buffsTransform);
                effect.Manager = this;
                effect.Refresh(duration);
                effects.Add(buff.Scriptable, effect);
            }

            if (ReferenceEquals(TokenBrowser.Instance.SelectedToken, Token))
                TokenBrowser.Instance.UpdateEffectsChanges(Token, buff);
        }
        
        public void RemoveEffect(Scriptable.BuffEffect buff)
        {
            if(!effects.ContainsKey(buff)) return;

            effects[buff].enabled = false;
        }
        
        public void RemoveExact(BuffEffect buff)
        {
            if(!effects.ContainsValue(buff)) return;

            effects[buff.Scriptable].enabled = false;
        }
    }
}