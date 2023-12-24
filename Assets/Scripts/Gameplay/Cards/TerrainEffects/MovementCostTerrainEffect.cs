using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Cards.TerrainEffects
{
    public class MovementCostTerrainEffect : TerrainEffect
    {
        [SerializeField] private int movementCost;
        
        protected override void OnApplied() => Manager.Card.OnMovementCostCollected += OnMovementCostCollected;
        protected override void OnRemoved() => Manager.Card.OnMovementCostCollected -= OnMovementCostCollected;
        private void OnDestroy() => OnRemoved();
        private void OnMovementCostCollected(List<int> costs) => costs.Add(movementCost);
        protected override void OnTick() { }
    }
}