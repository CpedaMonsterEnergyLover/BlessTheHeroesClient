using Gameplay.Abilities;
using Gameplay.GameField;
using Gameplay.Interaction;
using UI.Elements;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Tokens
{
    public interface IToken
    {
        public static IToken DraggedToken { get; private set; }
        
        
        
        // Methods
        public void SetCard(Card card);
        public void Damage(int damage, int delayMS = 200);
        
        
        // Properties
        public bool Initialized { get; }
        public Scriptable.DiceSet AttackDiceSet { get; }
        public Scriptable.DiceSet MagicDiceSet { get; }
        public Scriptable.DiceSet DefenseDiceSet { get; }
        public int AttackDiceAmount { get; }
        public int DefenseDiceAmount { get; }
        public Scriptable.Token ScriptableToken { get; }
        public Transform TokenTransform { get; }
        public Card TokenCard { get; }
        public int TokenActionPoints { get; }
        public InteractionLine InteractionLine { get; }
        public int CurrentMana { get; }
        public int MaxMana { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public Ability[] Abilities { get; }
        public RangedAttackVisualizer RangedAttackVisualizer { get; }



        
        // Events
        public delegate void TokenDragEvent(IToken token);
        public static event TokenDragEvent OnStartDragging;
        public static event TokenDragEvent OnEndDragging;

        public delegate void TokenEvent(IToken token);
        public event TokenEvent OnTokenDestroy;
        public event TokenEvent OnTokenDataChanged;

        public delegate void TokenAttackEvent(IToken executor, IToken target, AttackType attackType, int damage, int defensed);
        public event TokenAttackEvent OnAttackPerformed;

        public void InvokeStartDraggingEvent()
        {
            DraggedToken = this;
            OnStartDragging?.Invoke(this);
        }

        public void InvokeOnEndDraggingEvent()
        {
            DraggedToken = null;
            OnEndDragging?.Invoke(this);
        }
    }
}