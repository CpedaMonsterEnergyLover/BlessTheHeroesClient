using System.Collections.Generic;
using System.Linq;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Aggro
{
    public class ControllableAggroManager : AggroManager<IControllableToken, IUncontrollableToken>
    {
        private float aggro;
        public float AggroLevel
        {
            get => aggro;
            private set
            {
                aggro = Mathf.Clamp(value, 0, float.MaxValue);
                if(aggroText.enabled) ShowAggroText();
            }
        }
        
        
        
        // Unity methods
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
        private void OnAnyKeyDown(in HashSet<KeyCode> keyCodes)
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                ShowAggroText();
                keyCodes.Add(KeyCode.LeftAlt);
            } else if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                ShowAggroText();
                keyCodes.Add(KeyCode.RightAlt);
            }
        }

        private void OnKeyReleased(KeyCode keyCode)
        {
            if (keyCode is KeyCode.LeftAlt or KeyCode.RightAlt) aggroText.enabled = false;
        }

        private void ShowAggroText()
        {
            /*float collectedAggro = Token.TokenCard.AggroCollector.GetAggro();
            int percent = collectedAggro == 0 
                ? 0 
                : Mathf.RoundToInt(AggroLevel / collectedAggro * 100);
            aggroText.SetText($"{percent}%");*/
            aggroText.SetText($"{AggroLevel}");
            aggroText.enabled = true;
        }

        public override void AddAggro(int amount, IUncontrollableToken source)
        {
            if(source is not null && (source.TokenCard.GridPosition - Token.TokenCard.GridPosition).sqrMagnitude > 1) 
                return;
            
            AggroLevel += amount;
        }

        public void RemoveAggro(float amount)
        {
            AggroLevel = Mathf.Clamp(AggroLevel - amount, 0, float.MaxValue);
        }

        public override void ClearAggro()
        {
            AggroLevel = 0;
        }
        
        protected override void OnMove(IToken t, Card card)
        {
            var enemies = card.Creatures.Where(h => !h.Dead).ToArray();
            int enemiesCount = enemies.Length;
            var allies = card.Heroes.Where(h => !h.Dead).ToArray();
            
            if (enemiesCount == 0)
                AggroLevel = 0;
            else if (allies.Length == 0) 
                AddAggro(enemiesCount, null);
        }
    }
}