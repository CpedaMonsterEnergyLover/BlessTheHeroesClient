using Cysharp.Threading.Tasks;
using Gameplay.GameCycle;
using UnityEngine;

namespace Gameplay.Cards.TerrainEffects
{
    public abstract class TerrainEffect : MonoBehaviour
    {
        [SerializeField] private Scriptable.TerrainEffect scriptable;

        public Scriptable.TerrainEffect Scriptable => scriptable;
        public TerrainManager Manager { get; set; }
        public int Duration { get; private set; }


        
        protected abstract void OnApplied();
        protected abstract void OnRemoved();
        protected abstract void OnTick();

        protected async void OnEnable()
        {
            await UniTask.WaitUntil(() => Manager is not null);

            TurnManager.OnPlayersTurnStarted += OnTurnStart;
            OnApplied();
        }

        protected void OnDisable()
        {
            TurnManager.OnPlayersTurnStarted -= OnTurnStart;
            OnRemoved();
        }

        private void OnTurnStart() => Tick();

        public void Refresh(int duration)
        {
            Duration = duration;
        }
        
        private void Tick()
        {
            OnTick();
            Duration--;
            if (Duration == 0) Manager.RemoveExact(this);
        }
    }
}