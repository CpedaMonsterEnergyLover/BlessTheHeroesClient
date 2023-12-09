using System.Collections.Generic;
using System.Linq;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using MyBox;
using UnityEngine;
using Util.Interaction;

namespace Gameplay.Aggro
{
    public class UncontrollableAggroManager : AggroManager<IControllableToken, IUncontrollableToken>
    {
        [SerializeField] private Vector3 gizmosOffset;

        public UncontrollableAggroManager(Vector3 gizmosOffset)
        {
            this.gizmosOffset = gizmosOffset;
        }


        // Unity Methods
        protected override void OnEnable()
        {
            base.OnEnable();
            KeyListener.OnAnyKeyDown += OnAnyKeyDown;
            KeyListener.OnKeyReleased += OnKeyReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            KeyListener.OnAnyKeyDown -= OnAnyKeyDown;
            KeyListener.OnKeyReleased -= OnKeyReleased;
        }

        
        
        // Class methods
#if UNITY_EDITOR
        protected override Color GizmosColor => Color.red;
        protected override Vector3 GizmosOffset => new(-0.025f, 0, 0);
#endif
        protected override IUncontrollableToken[] GetAllies(Card card) => card.Creatures.Where(c => !c.Dead).ToArray();

        protected override IControllableToken[] GetEnemies(Card card) => card.Heroes.Where(c => !c.Dead).ToArray();


        
        private void OnAnyKeyDown(in HashSet<KeyCode> keyCodes)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                ShowAggroTarget();
                keyCodes.Add(KeyCode.LeftAlt);
            } else if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                ShowAggroTarget();
                keyCodes.Add(KeyCode.RightAlt);
            }
        }

        private void OnKeyReleased(KeyCode keyCode)
        {
            if (keyCode is KeyCode.LeftAlt or KeyCode.RightAlt)
            {
                Token.InteractionLine.Disable();
            }
        }

        private void ShowAggroTarget()
        {
            Token.InteractionLine.SetInteractableColor(InteractionState.Abandon);
            Token.InteractionLine.SetEnabled(GetAggroTarget(out var token), 
                token is null ? Vector3.zero : token.TokenTransform.position);
        }
        
        public bool GetAggroTarget(out IControllableToken token)
        {
            if (cluster.Count == 0)
            {
                token = null;
                return false;
            }

            var maxAggro = cluster.Max(e => e.Value);
            var candidates = cluster.Where(e => e.Value == maxAggro);
            var leastHealth = candidates.MinBy(e => e.Key.CurrentHealth);
            token = leastHealth.Key;
            return true;
        }
    }
}