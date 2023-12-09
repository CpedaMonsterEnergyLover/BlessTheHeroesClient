using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.Dice;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using Scriptable;
using Scriptable.AttackVariations;
using TMPro;
using UI;
using UnityEngine;
using Util;
using Util.Animators;
using Util.Enums;
using Util.Interaction;
using Util.Patterns;
using Util.Tokens;

namespace Gameplay.Tokens
{
    [RequireComponent(typeof(BuffManager))]
    public abstract partial class Token<T, TJ> : MonoBehaviour, IToken
        where T : Token
        where TJ : IAggroManager
    {
        [Header("TokenBase Fields")]
        [SerializeField] protected AttackAnimatorManager attackAnimatorManager;
        [SerializeField] private DamageAnimator damageAnimator;
        [SerializeField] protected InteractableOutline interactableOutline;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] protected TJ aggroManager;
        [SerializeField] private InteractionLine interactionLine;

        private readonly Ability[] abilities = new Ability[4];
        private Tween movementTween;

        protected int maxHealthBonus;
        protected int maxManaBonus;
        protected int spellPower;
        protected int attackPower;
        protected int defense;
        protected int speedBonus;
        
        protected abstract int DefaultActionPoints { get; }
        protected Card Card { get; set; }
        public abstract bool CanBeTargeted { get; }
        public T Scriptable { get; private set; }
        public IAggroManager IAggroManager => aggroManager;
        public bool IsPlayingAnimation => 
            damageAnimator.IsPlayingAnimation || 
            (movementTween is not null && movementTween.IsPlaying());
        public int CurrentHealth { get; private set; }
        public int MaxHealth => Scriptable.Health + maxHealthBonus;
        public int CurrentMana { get; private set; }
        public int MaxMana => Scriptable.Mana + maxManaBonus;
        public InteractionLine InteractionLine => interactionLine;



        // Unity methods
        private void Start()
        {
            PreInit().Forget();
        }

        private void OnEnable()
        {
            IToken.OnStartDragging += OnTokenStartDraggingEventInvoked;
            IToken.OnEndDragging += OnTokenEndDraggingEventInvoked;
        }

        private void OnDisable()
        {
            IToken.OnStartDragging -= OnTokenStartDraggingEventInvoked;
            IToken.OnEndDragging -= OnTokenEndDraggingEventInvoked;
        }

        protected virtual void OnDestroy()
        {
            IToken.OnStartDragging -= OnTokenStartDraggingEventInvoked;
            IToken.OnEndDragging -= OnTokenEndDraggingEventInvoked;
            TurnManager.OnPlayersTurnStarted -= OnPlayersTurnStarted;
            TurnManager.OnMonstersTurnStarted -= OnMonstersTurnStarted;
            OnDestroyed?.Invoke(this);
        }


        // Class methods
        protected abstract void Die();
        protected virtual void OnPlayersTurnStarted() { }
        protected virtual void OnMonstersTurnStarted() { }

        
        
        private async UniTaskVoid PreInit()
        {
            transform.localScale = Vector3.zero;
            await UniTask.WaitUntil(() => Scriptable is not null);
            Init();
            PostInit();
        }

        protected virtual void Init()
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            MovementPoints = Speed;
            ActionPoints = DefaultActionPoints;
            healthText.SetText(CurrentHealth.ToString());
            interactableOutline.SetOutlineWidth(GlobalDefinitions.TokenOutlineWidth);
            InstantiateAbilities();
            TurnManager.OnPlayersTurnStarted += OnPlayersTurnStarted;
            TurnManager.OnMonstersTurnStarted += OnMonstersTurnStarted;
        }

        private void PostInit()
        {
            TokenBrowser.Instance.SelectFirst(this);
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Initialized = true;
                UpdateOutlineByCanInteract();
            });
        }
        
        public void SetScriptable(T scriptable)
        {
            Scriptable = scriptable;
            spriteRenderer.sprite = scriptable.Sprite;
        }
        
        protected void InvokeDataChangedEvent() => OnStatsChanged?.Invoke(this);
        
        protected bool ConsumeActionPointForMovement()
        {
            if (ActionPoints <= 0) return false;
            ActionPoints--;
            MovementPoints += Speed;
            InvokeDataChangedEvent();
            return true;
        }
        
        public async UniTask Damage(int damage, Sprite overrideDamageSprite = null, IAggroManager aggroReceiver = null, int delay = 200)
        {
            if(Dead) return;
            Debug.Log($"Damaged for {damage} HP");
            var absorbed = OnDamageAbsorbed?.Invoke(damage);
            if(aggroReceiver is not null)
            {
                aggroReceiver.AddAggro(damage, this);
                aggroManager.AddAggro(damage, aggroReceiver.IToken);
            }
            
            int absorb = absorbed ?? 0;
            if (damage > 0 && absorb > 0)
            {
                await damageAnimator.PlayDamage(absorb, GlobalDefinitions.DefensedDamageAnimationSprite, delay);
                if (absorb >= damage)
                    return;
                
                damage -= absorb;
            }


            int health = CurrentHealth - damage;
            OnDamaged?.Invoke(damage);
            
            if (health <= 0)
            {
                Dead = true;
                await damageAnimator.PlayDamage(damage, overrideDamageSprite, delay);
                SetHealth(health);
                OnDeath?.Invoke(this);
                Die();
                await Despawn();
            }
            else
            {
                await damageAnimator.PlayDamage(damage, overrideDamageSprite, delay);
                SetHealth(health);
                
                if(IToken.DraggedToken is not null) 
                    interactableOutline.SetEnabled(CanBeTargeted && 
                                            IToken.DraggedToken.TokenActionPoints != 0 && 
                                            IsInAttackRange(IToken.DraggedToken));
            }
        }

        public virtual async UniTask Despawn()
        {
            Dead = true;
            transform.SetParent(null);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            Card.RemoveToken(this, false);
            Destroy(gameObject);
        }
        
        protected virtual async UniTask Walk(Card card)
        {
            SetMovementPoints(MovementPoints - 1);
            await Move(card);
        }

        public async UniTask Move(Card card)
        {
            Card previous = Card;
            OnMove?.Invoke(this, card);
            card.AddToken(this, except: true, instantly: false);
            movementTween = transform.DOLocalJump(
                    card.GetLastTokenPosition(this), 0.5f, 1, 0.5f)
                .OnComplete(() =>
                {
                    movementTween = null;
                    UpdateOutlineByCanInteract();
                });

            await UniTask.WaitUntil(() => movementTween == null);
            previous.RemoveToken(this, instantly: false);
        }
        
        private void InstantiateAbilities()
        {
            for (int i = 0; i < 4; i++)
            {
                var ability = Scriptable.Abilities[i];
                if (ability is not null)
                {
                    var inst = Instantiate(ability, transform);
                    inst.gameObject.name = ability.Title;
                    abilities[i] = inst;
                }
            }
            foreach (Ability ability in abilities)
            {
                if(ability is not null)
                    ability.SetToken(this);
            }
        }

        public bool IsInAttackRange(IToken attacker)
        {
            var cards = attacker.GetCardsInAttackRange();
            return cards.Contains(Card);
        }
        
        public List<Card> GetCardsInAttackRange()
        {
            List<Card> cards = new();

            switch (AttackType)
            {
                case AttackType.Melee:
                    cards.Add(Card);
                    break;
                case AttackType.Ranged:
                {
                    PatternSearch.IterateNeighbours(Card.GridPosition, pos => {
                        if(FieldManager.GetCard(pos, out Card card)) cards.Add(card);
                    });
                    break;
                }
                case AttackType.Magic:
                    cards.Add(Card);
                    PatternSearch.IterateNeighbours(Card.GridPosition, pos => {
                        if(FieldManager.GetCard(pos, out Card card)) cards.Add(card);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return cards;
        }

        private void OnTokenStartDraggingEventInvoked(IToken token) => interactableOutline.SetEnabled(false);

        private void OnTokenEndDraggingEventInvoked(IToken token) => UpdateOutlineByCanInteract();


        // IToken
        public GameObject GameObject => gameObject;
        public bool Dead { get; private set; }
        public abstract DiceSet AttackDiceSet { get; }
        public abstract DiceSet MagicDiceSet { get; }
        public abstract DiceSet DefenseDiceSet { get; }
        public abstract int AttackDiceAmount { get; }
        public abstract int DefenseDiceAmount { get; }
        public abstract BaseAttackVariation AttackVariation { get; }
            public bool Initialized { private set; get; }
        public event IToken.TokenEvent OnDestroyed;
        public event IToken.TokenEvent OnStatsChanged;
        public event IToken.TokenEvent OnActionsChanged;
        public event IToken.TokenEvent OnHealthChanged;
        public event IToken.TokenEvent OnManaChanged;
        public event IToken.TokenAttackEvent OnBeforeAttackPerformed;
        public event IToken.TokenAttackEvent OnAfterAttackPerformed;
        public int Speed => Scriptable.Speed + speedBonus;
        public AttackType AttackType => Scriptable.AttackType;
        public InteractableOutline InteractableOutline => interactableOutline;
        public Card TokenCard => Card;
        Token IToken.ScriptableToken => Scriptable;
        public Transform TokenTransform => transform;
        public int TokenActionPoints => ActionPoints;
        public void SetCard(Card card) => Card = card;
        public Ability[] Abilities => abilities.ToArray();
        public AttackAnimatorManager AttackAnimatorManager => attackAnimatorManager;
        public int ActionPoints { get; protected set; }
        public int MovementPoints { get; protected set; }
        public int SpellPower => spellPower;
        public int AttackPower => attackPower;
        public int Defense => defense;
        public BuffManager BuffManager => buffManager;

        public bool HasAbility(string id, out Ability ability)
        {
            ability = abilities.FirstOrDefault(a => string.Equals(a.Title, id));
            return ability is not null;
        }
        
        public async UniTask Attack(IToken target)
        {
            AttackAnimatorManager.StartAnimation(transform, AttackType, AttackVariation, target.TokenTransform.position);

            bool magicAttack = AttackType is AttackType.Magic;
            
            int damage;
            int attackEnergy;
            int[] attackSides;
            int[] defenseSides = new int[3];
            int defensed = 0;
            
            DiceSet attackDice = magicAttack ? MagicDiceSet : AttackDiceSet; 
            
            bool hit = magicAttack
                ? DiceUtil.CaclulateMagicDiceThrow(AttackDiceAmount, attackDice, SpellPower,
                        out damage, out attackEnergy, out attackSides)
                : DiceUtil.CalculateAttackDiceThrow(AttackDiceAmount, attackDice, AttackPower,
                out damage, out attackEnergy, out attackSides);
            
            bool def = !magicAttack && DiceUtil.CalculateDefenseDiceThrow(target.DefenseDiceAmount, target.DefenseDiceSet, target.Defense, 
                out defensed, out _, out defenseSides);
            
            if (this is IUncontrollableToken)
            {
                await DiceManager.ThrowReplay(
                    target.DefenseDiceSet, target.DefenseDiceAmount, 
                    defenseSides.Concat(attackSides).ToArray(),
                    attackDice, AttackDiceAmount);
            } 
            else if (this is IControllableToken)
            {
                await DiceManager.ThrowReplay(
                    attackDice, AttackDiceAmount, 
                    attackSides.Concat(defenseSides).ToArray(),
                    target.DefenseDiceSet, target.DefenseDiceAmount);
                EnergyManager.Instance.AddEnergy(this, attackEnergy);
            }
            
            Debug.Log($"Hit: {hit}, Damage: {damage}, Defensed: {defensed}");
            AttackAnimatorManager.StopAnimation(transform, AttackType);
            if(!hit)
            {
                ((IToken) this).InvokeOnTokenMissGlobal();
                return;
            }
            
            int finalDamage = def ? Mathf.Clamp(damage - defensed, 0, int.MaxValue) : damage;
            OnBeforeAttackPerformed?.Invoke(this, target, Scriptable.AttackType, damage, defensed);
            await UniTask.WhenAll(
                target.Damage(finalDamage, aggroManager: aggroManager), 
                AttackAnimatorManager.AnimateAttack(transform, AttackType, target.TokenTransform)
            );
            AttackAnimatorManager.StopAnimation(transform, AttackType);
            OnAfterAttackPerformed?.Invoke(this, target, Scriptable.AttackType, damage, defensed);
            UpdateOutlineByCanInteract();
        }
        
        public bool Push(Card card)
        {
            Card previous = Card;
            if (!card.HasSpaceForToken(this)) return false;
            card.AddToken(this, instantly: true);
            // TODO: Animations xd
            // animationTween = transform.DOLocalMove(card.GetLastTokenPosition(this), 0.25f);
            previous.RemoveToken(this, instantly: true);
            movementTween = null;
            UpdateOutlineByCanInteract();
            
            return true;
        }
        

        // IInteractable
        public abstract bool CanInteract { get; }
        public abstract Vector4 OutlineColor { get; }
        public abstract void UpdateOutlineByCanInteract();
        public InteractableOutline Outline => interactableOutline;


        // IInteractableOnClick
        public abstract bool CanClick { get; }
        public void OnClick(InteractionResult result)
        {
            TokenBrowser.Instance.SelectToken(this);
        }
    }
}