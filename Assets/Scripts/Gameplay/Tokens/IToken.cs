using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Interaction;
using Scriptable.AttackVariations;
using UI.Elements;
using UnityEngine;
using Util.Animators;
using Util.Enums;
using Util.Interaction;

namespace Gameplay.Tokens
{
    public interface IToken : IInteractableOnClick
    {
        public static IToken DraggedToken { get; private set; }
        
        
        
        // Methods
        public List<Card> GetCardsInAttackRange();
        public bool DrainMana(int amount);
        public void SetCard(Card card);
        public UniTask Attack(IToken target);
        public UniTask Move(Card card);
        public bool Push(Card card);
        public UniTask Despawn();
        public bool IsInAttackRange(IToken attacker);
        public void InvokeOnTokenMissGlobal() => OnTokenMissGlobal?.Invoke(this);


        
        // Properties
        public InteractionLine InteractionLine { get; }
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
        public InteractableOutline InteractableOutline { get; }
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
        public AttackAnimatorManager AttackAnimatorManager { get; }
        public BuffManager BuffManager { get; }
        public IAggroManager IAggroManager { get; }
        public BaseAttackVariation AttackVariation { get; }
        
        
        // API
        public UniTask Damage(int damage, Sprite overrideDamageSprite = null, IAggroManager aggroManager = null, int delay = 200);
        public void Heal(int health, IAggroManager aggroManager = null);
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
        public delegate void TokenEvent(IToken token);
        public static event TokenEvent OnStartDragging;
        public static event TokenEvent OnEndDragging;
        public static event TokenEvent OnTokenMissGlobal;
        public event TokenEvent OnDestroyed;
        public event TokenEvent OnStatsChanged;
        public event TokenEvent OnDeath;
        public event TokenEvent OnManaChanged;
        public event TokenEvent OnHealthChanged;
        public event TokenEvent OnActionsChanged;

        public delegate void TokenAttackEvent(IToken executor, IToken target, AttackType attackType, int damage, int defensed);
        public event TokenAttackEvent OnBeforeAttackPerformed;
        public event TokenAttackEvent OnAfterAttackPerformed;

        public delegate int TokenDamageAbsorbtionEvent(int damage);
        public event TokenDamageAbsorbtionEvent OnDamageAbsorbed;
        
        public delegate void TokenResourceEvent(int amount);
        public event TokenResourceEvent OnDamaged;
        public event TokenResourceEvent OnHealed;
        public event TokenResourceEvent OnManaReplenished;

        
        public delegate void TokenMoveEvent(IToken t, Card card);
        public event TokenMoveEvent OnMove;


        public void InvokeStartDraggingEvent()
        {
            DraggedToken = this;
            OnStartDragging?.Invoke(this);
        }

        public void InvokeEndDraggingEvent()
        {
            DraggedToken = null;
            OnEndDragging?.Invoke(this);
        }
    }
}