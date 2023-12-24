using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using UI.Browsers;
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
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] protected TJ aggroManager;
        [SerializeField] private InteractionLine interactionLine;
        [SerializeField] private Transform abilitiesTransform;
        [SerializeField] private InteractableOutline interactableOutline;

        private readonly List<Ability> abilities = new();
        protected bool dead;
        private Tween movementTween;
        protected int maxHealthBonus;
        protected int maxManaBonus;
        protected int spellPower;
        protected int attackPower;
        protected int defense;
        protected int speedBonus;
        
        public abstract BaseAttackVariation AttackVariation { get; }
        public T Scriptable { get; private set; }
        public virtual bool CanAct => CanBeTargeted && ActionPoints > 0;
        public virtual bool CanWalk => CanBeTargeted && MovementPoints > 0;
        public virtual bool CanAttack => CanAct && AttackDiceAmount > 0;
        public virtual bool CanCast => CanAct;
        protected abstract int DefaultActionPoints { get; }
        protected bool IsPlayingAnimation => movementTween is not null && movementTween.IsPlaying();
        protected Card previousCard;



        private void Start() => PreInit().Forget();

        protected virtual void OnDestroy()
        {

            TurnManager.OnPlayersTurnStarted -= OnPlayersTurnStarted;
            TurnManager.OnMonstersTurnStarted -= OnMonstersTurnStarted;
            OnDestroyed?.Invoke(this);
        }
        
        protected abstract void Die(IToken attacker);

        protected virtual void OnPlayersTurnStarted()
        {
            SetActionPoints(DefaultActionPoints);
            SetMovementPoints(Speed);
        }
        protected virtual void OnMonstersTurnStarted() { }
        
        private async UniTaskVoid PreInit()
        {
            transform.localScale = Vector3.zero;
            await UniTask.WaitUntil(() => Scriptable is not null && Card is not null);
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
            InstantiateAbilities();
            TurnManager.OnPlayersTurnStarted += OnPlayersTurnStarted;
            TurnManager.OnMonstersTurnStarted += OnMonstersTurnStarted;
        }

        private void PostInit()
        {
            if(TokenBrowser.SelectedToken is null) TokenBrowser.SelectToken(this);
            aggroManager.Activate(this);
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Initialized = true;
                OnInitialized?.Invoke(this);
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
            if (!CanAct) return false;
            SetActionPoints(ActionPoints - 1);
            MovementPoints += Speed;
            return true;
        }
        
        public async UniTask Damage(DamageType damageType, int damage, IToken attacker, bool useAttackerPosition = true, int delay = 200)
        {
            if(damageType is null || Dead) return;
            var delaySpan = TimeSpan.FromMilliseconds(delay);
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();

            
            Transform sourceTransform = attacker is not null && useAttackerPosition 
                ? attacker.TokenTransform
                : null;
            var absorbed = OnDamageAbsorbed?.Invoke(damage);

            int absorb = absorbed ?? 0;
            if (damage > 0 && absorb > 0)
            {
                await UniTask.Delay(delaySpan, cancellationToken: token);
                await damageAnimator.PlayDamageAsync(absorb, damageType, DamageImpact.Absorb, sourceTransform);
                if (absorb >= damage)
                    return;
                
                damage -= absorb;
            }

            DamageImpact impact = ImpactDamage(damageType, damage, out damage);
            
            if(attacker is not null) 
                attacker.BaseAggroManager.AddAggro(damage, this);
            
            int health = CurrentHealth - damage;
            OnDamaged?.Invoke(damageType, damage);

            if (health <= 0)
            {
                dead = true;
                await UniTask.Delay(delaySpan, cancellationToken: token);
                SetHealth(health);
                await damageAnimator.PlayDamageAsync(damage, damageType, impact, sourceTransform);
                OnDeath?.Invoke(this);
                Die(attacker);
                await Despawn();
            }
            else
            {
                await UniTask.Delay(delaySpan, cancellationToken: token);
                SetHealth(health);
                await damageAnimator.PlayDamageAsync(damage, damageType, impact, sourceTransform);
            }
        }

        private DamageImpact ImpactDamage(DamageType damageType, int damage, out int impacted)
        {
            impacted = damage;
            CreatureType creatureType = Scriptable.CreatureType;
            
            if (creatureType.ResistantTo(damageType))
            {
                impacted = Mathf.Clamp(damage - Mathf.CeilToInt(damage * 0.5f) , 0, int.MaxValue);
                return DamageImpact.Resist;
            }

            if (creatureType.VulnerableTo(damageType))
            {
                impacted = Mathf.Clamp(damage + Mathf.CeilToInt(damage * 0.5f) , 0, int.MaxValue);
                return DamageImpact.Vulnerable;
            }
            
            return DamageImpact.Normal;
        }

        public virtual async UniTask Despawn()
        {
            dead = true;
            transform.SetParent(null);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            Card.RemoveToken(this, false);
            Destroy(gameObject);
        }
        
        protected virtual async UniTask Walk(Card card)
        {
            if (MovementPoints < card.MovementCost && !ConsumeActionPointForMovement()) return;
            
            SetMovementPoints(MovementPoints - card.MovementCost);
            await Move(card);
        }

        protected bool CanWalkOnCard(Card card)
        {
            if (!card.IsOpened || !CanWalk || !card.HasSpaceForToken(this)) return false;
            
            int cost = card.MovementCost;
            return MovementPoints >= cost || (MovementPoints + Speed >= cost && CanAct);
        }
        
        public async UniTask Move(Card card)
        {
            previousCard = Card;
            card.AddToken(this, except: true, instantly: false);
            OnMove?.Invoke(this, card);
            movementTween = transform.DOLocalJump(
                    card.GetLastTokenPosition(this), 0.5f, 1, 0.5f)
                .OnComplete(() =>
                {
                    movementTween = null;
                    // UpdateOutlineByCanInteract();
                });

            await UniTask.WaitUntil(() => movementTween == null);
            previousCard.RemoveToken(this, instantly: false);
        }
        
        private void InstantiateAbilities()
        {
            foreach (Ability ability in Scriptable.Abilities)
            {
                var inst = Instantiate(ability, abilitiesTransform);
                inst.gameObject.name = ability.Title;
                abilities.Add(inst);
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
        

        // IToken
        public abstract DiceSet AttackDiceSet { get; }
        public abstract DiceSet MagicDiceSet { get; }
        public abstract DiceSet DefenseDiceSet { get; }
        public abstract DamageType DamageType { get; }
        public abstract int AttackDiceAmount { get; }
        public abstract int DefenseDiceAmount { get; }
        public bool Dead => dead;
        public bool Initialized { private set; get; }
        public int Speed => Scriptable.Speed + speedBonus;
        public AttackType AttackType => Scriptable.AttackType;
        public ArmorType ArmorType => Scriptable.ArmorType;
        public Card Card { get; set; }
        Token IToken.ScriptableToken => Scriptable;
        public Transform TokenTransform => transform;
        public int TokenActionPoints => ActionPoints;
        public Ability[] Abilities => abilities.ToArray();
        public AttackAnimatorManager AttackAnimatorManager => attackAnimatorManager;
        public int ActionPoints { get; private set; }
        public int MovementPoints { get; private set; }
        public int SpellPower => spellPower;
        public int AttackPower => attackPower;
        public int Defense => defense;
        public BuffManager BuffManager => buffManager;
        public IAggroManager BaseAggroManager => aggroManager;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => Scriptable.Health + maxHealthBonus;
        public int CurrentMana { get; private set; }
        public int MaxMana => Scriptable.Mana + maxManaBonus;
        public InteractionLine InteractionLine => interactionLine;
        public bool CanBeTargeted => Initialized && !Dead;


        public event IInteractable.InteractableEvent OnDestroyed;
        public event IInteractable.InteractableEvent OnInitialized;
        public event IToken.TokenEvent OnStatsChanged;
        public event IToken.TokenEvent OnActionsChanged;
        public event IToken.TokenEvent OnMovementPointsChanged;
        public event IToken.TokenEvent OnHealthChanged;
        public event IToken.TokenEvent OnManaChanged;
        public event IToken.TokenEvent OnDeath;
        public event IToken.TokenAttackEvent OnBeforeAttackPerformed;
        public event IToken.TokenAttackEvent OnAfterAttackPerformed;
        public event IToken.TokenDamageEvent OnDamaged;
        public event IToken.TokenResourceEvent OnHealed;
        public event IToken.TokenResourceEvent OnManaReplenished;
        public event IToken.TokenMoveEvent OnMove;
        public event IToken.TokenDamageAbsorbtionEvent OnDamageAbsorbed;

        public bool HasAbility(string id, out Ability ability)
        {
            ability = abilities.FirstOrDefault(a => string.Equals(a.Title, id));
            return ability is not null;
        }
        
        public async UniTask Attack(IToken target)
        {
            AttackAnimatorManager.StartAnimation(transform, AttackType, AttackVariation, target.TokenTransform.position);

            bool magicAttack = DamageType.Origin is DamageType.DamageOrigin.Magic;
            
            int damage;
            int attackEnergy;
            int[] attackSides;
            int[] defenseSides = Array.Empty<int>();
            int defensed = 0;
            
            DiceSet attackDice = magicAttack ? MagicDiceSet : AttackDiceSet; 
            
            bool hit = magicAttack
                ? DiceUtil.CaclulateMagicDiceThrow(AttackDiceAmount, attackDice, SpellPower,
                        out damage, out attackEnergy, out attackSides)
                : DiceUtil.CalculateAttackDiceThrow(AttackDiceAmount, attackDice, AttackPower,
                out damage, out attackEnergy, out attackSides);
            
            bool def = !magicAttack && DiceUtil.CalculateDefenseDiceThrow(
                target.DefenseDiceAmount, target.DefenseDiceSet, target.Defense, 
                out defensed, out defenseSides);
            
            if (this is IUncontrollableToken)
            {
                await DiceManager.ThrowReplay(
                    def ? target.DefenseDiceSet : null, 
                    def ? target.DefenseDiceAmount : 0, 
                    defenseSides.Concat(attackSides).ToArray(),
                    attackDice, AttackDiceAmount);
            } 
            else if (this is IControllableToken)
            {
                await DiceManager.ThrowReplay(
                    attackDice, AttackDiceAmount, 
                    attackSides.Concat(defenseSides).ToArray(),
                    def ? target.DefenseDiceSet : null, 
                    def ? target.DefenseDiceAmount : 0);
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
                target.Damage(DamageType, finalDamage, this, delay: AttackVariation.DamageDelay), 
                AttackAnimatorManager.AnimateAttack(transform, AttackType, target.TokenTransform)
            );
            AttackAnimatorManager.StopAnimation(transform, AttackType);
            OnAfterAttackPerformed?.Invoke(this, target, Scriptable.AttackType, damage, defensed);
            // UpdateOutlineByCanInteract();
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
            // UpdateOutlineByCanInteract();
            OnMove?.Invoke(this, card);
            return true;
        }
        

        // IInteractable
        public abstract bool CanInteract { get; }
        public abstract Vector4 OutlineColor { get; }
        public InteractableOutline InteractableOutline => interactableOutline;


        // IInteractableOnClick
        public abstract bool CanClick { get; }
        public void OnClick(InteractionResult result, int clickCount)
        {
            TokenBrowser.SelectToken(this);
        }
    }
}