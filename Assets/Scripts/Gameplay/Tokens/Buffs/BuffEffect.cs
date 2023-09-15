﻿using Cysharp.Threading.Tasks;
using Gameplay.GameCycle;
using UnityEngine;

namespace Gameplay.Tokens.Buffs
{
    public abstract class BuffEffect : MonoBehaviour
    {
        [SerializeField] private Scriptable.BuffEffect scriptable;

        public Scriptable.BuffEffect Scriptable => scriptable;

        public BuffManager Manager { get; set; }
        public int Duration { get; private set; }

        public delegate void BuffEffectEvent(BuffEffect effect);
        public event BuffEffectEvent OnDurationChanged;
        public event BuffEffectEvent OnStatusChanged;

        

        protected async void OnEnable()
        {
            await UniTask.WaitUntil(() => Manager.Token is not null);
            
            if (Manager.Token is IControllableToken)
                TurnManager.OnPlayersTurnStarted += OnTurnStart;
            else
                TurnManager.OnMonstersTurnStarted += OnTurnStart;
            OnStatusChanged?.Invoke(this);
            OnApplied();
        }

        protected void OnDisable()
        {
            TurnManager.OnPlayersTurnStarted -= OnTurnStart;
            TurnManager.OnMonstersTurnStarted -= OnTurnStart;
            OnStatusChanged?.Invoke(this);
            OnRemoved();
        }

        private void OnTurnStart()
        {
            Tick();
        }
        
        public void Refresh(int duration)
        {
            Duration = duration;
            OnDurationChanged?.Invoke(this);
        }

        protected abstract void OnApplied();
        protected abstract void OnRemoved();
        protected abstract void OnTick();

        private void Tick()
        {
            Duration--;
            OnDurationChanged?.Invoke(this);
            if (Duration == 0) 
                Manager.RemoveExact(this);
            else OnTick();
        }
    }
}