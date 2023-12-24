using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Cards.TerrainEffects
{
    [RequireComponent(typeof(Card))]
    public class TerrainManager : MonoBehaviour
    {
        public Card Card { get; private set; }
        public IEnumerable<TerrainEffect> ActiveEffects => effects.Values.Where(e => e.isActiveAndEnabled);
        
        private Transform effectsTransform;
        private readonly Dictionary<Scriptable.TerrainEffect, TerrainEffect> effects = new();

        

        private void Awake()
        {
            Card = GetComponent<Card>();
            effectsTransform = new GameObject("TerrainEffects").transform;
            effectsTransform.SetParent(transform);
        }
        
        public void ApplyEffect(TerrainEffect terrainEffect, int duration)
        {
            var scriptable = terrainEffect.Scriptable;
            if (effects.ContainsKey(scriptable))
            {
                var effect = effects[scriptable];
                effect.gameObject.SetActive(true);
                effect.Refresh(duration);
            }
            else
            {
                var effect = Instantiate(terrainEffect, effectsTransform);
                effect.Manager = this;
                effect.Refresh(duration);
                effects.Add(scriptable, effect);
            }
        }
        
        public void RemoveExact(TerrainEffect effect)
        {
            if(!effects.ContainsValue(effect)) return;

            effects[effect.Scriptable].gameObject.SetActive(false);
        }
    }
}