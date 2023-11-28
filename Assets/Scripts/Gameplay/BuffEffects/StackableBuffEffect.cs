using UnityEngine;

namespace Gameplay.BuffEffects
{
    public abstract class StackableBuffEffect : BuffEffect
    {
        [SerializeField] private int maxStacks;

        public delegate void StacksUpdateEvent(int stacks);

        public event StacksUpdateEvent OnStacksUpdated;
        
        
        
        public int Stacks { get; protected set; }

        protected override void OnApplied() { }

        protected override void OnRemoved() => Stacks = 0;

        protected abstract void OnStacksChanged(int previousStacks, int newStacks);

        public override void Refresh(int duration)
        {
            base.Refresh(duration);
            int prev = Stacks;
            Stacks = Mathf.Clamp(Stacks + 1, 1, maxStacks);
            if(prev != Stacks)
            {
                OnStacksChanged(prev, Stacks);
                OnStacksUpdated?.Invoke(Stacks);
            }
        }
    }
}