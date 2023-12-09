using System.Linq;
using Gameplay.Cards;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Aggro
{
    public class ControllableAggroManager : AggroManager<IUncontrollableToken, IControllableToken>
    {
#if UNITY_EDITOR
        protected override Color GizmosColor => Color.green;
        protected override Vector3 GizmosOffset => new(0.025f, 0, 0);
#endif
        protected override IControllableToken[] GetAllies(Card card) => card.Heroes.Where(c => !c.Dead).ToArray();
        
        protected override IUncontrollableToken[] GetEnemies(Card card) => card.Creatures.Where(c => !c.Dead).ToArray();
    }
}