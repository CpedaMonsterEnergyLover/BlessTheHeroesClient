using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.Interaction;
using Scriptable;
using Scriptable.AttackVariations;
using UnityEngine;
using Util.Animators;
using Util.Enums;

namespace Gameplay.Tokens
{
    public interface IToken : IInteractableOnClick
    {
        // Methods
        public List<Card> GetCardsInAttackRange();
        public bool DrainMana(int amount);
        public UniTask Attack(IToken target);
        public UniTask Move(Card card);
        public bool Push(Card card);
        public UniTask Despawn();
        public bool IsInAttackRange(IToken attacker);
        public void InvokeOnTokenMissGlobal() => OnTokenMissGlobal?.Invoke(this);
        public bool HasAbility(string id, out Ability ability);
        
        
        // API
        public UniTask Damage(DamageType damageType, int damage, IToken attacker, bool useAttackerPosition = true, int delay = 200);
        public void Heal(DamageType healType, int health, IToken caster, bool useCasterPosition = true);
        public void ReplenishMana(int mana);
        public void SetActionPoints(int amount);
        public void SetMovementPoints(int amount);
        public void AddSpellPower(int amount);
        public void AddSpeed(int amount);
        public void AddDefense(int amount);
        public void AddAttackPower(int amount);
        public void AddMaxMana(int amount);
        public void AddMaxHealth(int amount);

        
        // Properties
        public InteractionLine InteractionLine { get; }
        public bool Initialized { get; }
        public int ActionPoints { get; }
        public int MovementPoints { get; }
        public AttackType AttackType { get; }
        public ArmorType ArmorType { get; }
        public int Speed { get; }
        public DiceSet AttackDiceSet { get; }
        public DiceSet MagicDiceSet { get; }
        public DiceSet DefenseDiceSet { get; }
        public DamageType DamageType { get; }
        public int AttackDiceAmount { get; }
        public int DefenseDiceAmount { get; }
        public Token ScriptableToken { get; }
        public Transform TokenTransform { get; }
        public Card Card { get; set; }
        public int TokenActionPoints { get; }
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int CurrentHealth { get; }
        public int CurrentMana { get; }
        public int SpellPower { get; }
        public int AttackPower { get; }
        public int Defense { get; }
        public Ability[] Abilities { get; }
        public AttackAnimatorManager AttackAnimatorManager { get; }
        public BuffManager BuffManager { get; }
        public IAggroManager BaseAggroManager { get; }
        public bool CanBeTargeted { get; }
        public bool CanCast { get; }
        public bool CanAct { get; }
        
        

        // Events
        public delegate void TokenEvent(IToken token);
        public static event TokenEvent OnTokenMissGlobal;
        public event TokenEvent OnStatsChanged;
        public event TokenEvent OnDeath;
        public event TokenEvent OnManaChanged;
        public event TokenEvent OnHealthChanged;
        public event TokenEvent OnActionsChanged;
        public event TokenEvent OnMovementPointsChanged;
        

        public delegate void TokenAttackEvent(IToken executor, IToken target, AttackType attackType, int damage, int defensed);
        public event TokenAttackEvent OnBeforeAttackPerformed;
        public event TokenAttackEvent OnAfterAttackPerformed;

        public delegate int TokenDamageAbsorbtionEvent(int damage);
        public event TokenDamageAbsorbtionEvent OnDamageAbsorbed;

        public delegate void TokenDamageEvent(DamageType damageType, int damage);
        public event TokenDamageEvent OnDamaged;

        public delegate void TokenResourceEvent(int amount);
        public event TokenResourceEvent OnHealed;
        public event TokenResourceEvent OnManaReplenished;

        
        public delegate void TokenMoveEvent(IToken t, Card card);
        public event TokenMoveEvent OnMove;
    }
}