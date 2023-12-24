using System.Collections.Generic;
using System.Linq;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    public abstract class Item : ScriptableObject
    {
        [Header("Item data")] 
        [SerializeField] private new string name;
        [SerializeField] private string description;
        [SerializeField] private int price;
        [SerializeField] private Sprite sprite;

        public string Description => description;
        public int Price => price;
        public string Name => name;
        public Sprite Sprite => sprite;
        
        
        
        public abstract bool AllowClick { get; }
        public abstract int StackSize { get; }
        public abstract string CategoryName { get; }
        public abstract void Consume();

        public void GetTradeTargets(List<IInteractable> targets)
        {
            if (TokenBrowser.SelectedToken is HeroToken hero)
                targets.AddRange(hero.Card.Heroes.Where(h => h is HeroToken && !h.Equals(hero)));
        }
    }
}