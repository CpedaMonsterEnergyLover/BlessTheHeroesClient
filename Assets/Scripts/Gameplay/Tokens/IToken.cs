using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.GameField;
using Gameplay.Tokens.Buffs;
using UI.Elements;
using UnityEngine;
using Util.Enums;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public interface IToken
    {
        public static IToken DraggedToken { get; private set; }
        
        
        
        // Methods
        public void SetCard(Card card);
        public UniTask Attack(IToken target);
        public bool Push(Card card);
        public UniTaskVoid Despawn();
        public bool IsInAttackRange(IToken attacker);
        
        
        // Properties
        public GameObject GameObject { get; }
        public bool Initialized { get; }
        public int ActionPoints { get; }
        public int MovementPoints { get; }
        public AttackType AttackType { get; }
        public int Speed { get; }
        public Scriptable.DiceSet AttackDiceSet { get; }
        public Scriptable.DiceSet MagicDiceSet { get; }
        public Scriptable.DiceSet DefenseDiceSet { get; }
        public int AttackDiceAmount { get; }
        public int DefenseDiceAmount { get; }
        public Scriptable.Token ScriptableToken { get; }
        public Transform TokenTransform { get; }
        public TokenOutline TokenOutline { get; }
        public Card TokenCard { get; }
        public int TokenActionPoints { get; }
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int CurrentHealth { get; }
        public int CurrentMana { get; }
        public int SpellPower { get; }
        public int AttackPower { get; }
        public int Defense { get; }
        public bool Dead { get; }
        public Ability[] Abilities { get; }
        public bool HasAbility(string id, out Ability ability);
        public RangedAttackVisualizer RangedAttackVisualizer { get; }
        public BuffManager BuffManager { get; }
        
        
        // API
        public void Damage(int damage, int delayMS = 200, Sprite overrideDamageSprite = null);
        public void Heal(int health);
        public void ReplenishMana(int mana);
        public void SetActionPoints(int amount);
        public void SetMovementPoints(int amount);
        public void AddSpellPower(int amount);
        public void AddSpeed(int amount);
        public void AddDefense(int amount);
        public void AddAttackPower(int amount);
        public void AddMaxMana(int amount);
        public void AddMaxHealth(int amount);



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