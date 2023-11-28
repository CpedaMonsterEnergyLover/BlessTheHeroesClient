using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.BuffEffects;
using Gameplay.Dice;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using TMPro;
using UI;
using UI.Elements;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Util.Enums;
using Util.Interaction;
using Util.Patterns;
using Util.Tokens;

namespace Gameplay.Tokens
{
    [RequireComponent(typeof(BuffManager))]
    public abstract partial class Token<T, TJ> : MonoBehaviour, IToken
        where T : Scriptable.Token
        where TJ : IAggroManager
    {
        [Header("TokenBase Fields")]
        [SerializeField] protected RangedAttackVisualizer rangedAttackVisualizer;
        [SerializeField] private DamageAnimator damageAnimator;
        [FormerlySerializedAs("tokenOutline")] [SerializeField]
        protected InteractableOutline interactableOutline;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] private BuffManager buffManager;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] protected TJ aggroManager;

        private readonly Ability[] abilities = new Ability[4];

        protected int maxHealthBonus;
        protected int maxManaBonus;
        protected int spellPower;
        protected int attackPower;
        protected int defense;
        protected int speedBonus;
        protected Tween animationTween;


        protected abstract int DefaultActionPoints { get; }
        protected Card Card { get; set; }
        public abstract bool CanBeTargeted { get; }
        public T Scriptable { get; private set; }
        public IAggroManager IAggroManager => aggroManager;
        public bool IsPlayingAnimation => 
            damageAnimator.IsPlayingAnimation || 
            (animationTween is not null && animationTween.IsPlaying());
        public int CurrentHealth { get; protected set; }
        public int MaxHealth => Scriptable.Health + maxHealthBonus;
        public int CurrentMana { get; protected set; }
        public int MaxMana => Scriptable.Mana + maxManaBonus;

        

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
        
        private async UniTask DamageAsync(int damage, int absorb, int delayMS, Sprite overrideDamageSprite)
        {
            if (damage > 0 && absorb > 0)
            {
                await damageAnimator.PlayDamage(absorb, delayMS, GlobalDefinitions.DefensedDamageAnimationSprite);
                if (absorb >= damage)
                    return;
                
                damage -= absorb;
            }


            int health = CurrentHealth - damage;
            OnDamaged?.Invoke(damage);
            
            if (health <= 0)
            {
                Dead = true;
                await damageAnimator.PlayDamage(damage, delayMS, overrideDamageSprite);
                SetHealth(health);
                OnDeath?.Invoke(this);
                Die();
                await Despawn();
            }
            else
            {
                await damageAnimator.PlayDamage(damage, delayMS, overrideDamageSprite);
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
            animationTween = transform.DOLocalJump(
                    card.GetLastTokenPosition(this), 0.5f, 1, 0.5f)
                .OnComplete(() =>
                {
                    animationTween = null;
                    UpdateOutlineByCanInteract();
                });

            await UniTask.WaitUntil(() => animationTween == null);
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

        private async UniTask PlayAttackAnimation(Transform target)
        {
            if (AttackType == AttackType.Ranged)
            {
                rangedAttackVisualizer.SetArrowActive(false);
                await rangedAttackVisualizer.Shoot(target);
            }
            else
            {
                Vector3 prevPos = transform.localPosition;
                Vector3 direction = target.position - transform.position + prevPos + new Vector3(0, 0.1f, 0);
                direction -= direction.normalized * 0.25f;
                animationTween = DOTween.Sequence()
                    .Append(transform.DOLocalJump(direction, 0.25f, 1, 0.2f))
                    .Append(transform.DOLocalJump(prevPos, 0.5f, 1, 0.8f));
                await animationTween.AsyncWaitForKill();
                animationTween = null;
            }
        }
        
        public bool IsInAttackRange(IToken attacker)
        {
            return attacker.AttackType switch
            {
                AttackType.Melee => attacker.TokenCard == Card,
                AttackType.Ranged => PatternSearch.CheckNeighbours(
                    attacker.TokenCard.GridPosition,
                    Card.GridPosition),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OnTokenStartDraggingEventInvoked(IToken token) => interactableOutline.SetEnabled(false);

        private void OnTokenEndDraggingEventInvoked(IToken token) => UpdateOutlineByCanInteract();


        // IToken
        public GameObject GameObject => gameObject;
        public bool Dead { get; private set; }
        public abstract Scriptable.DiceSet AttackDiceSet { get; }
        public abstract Scriptable.DiceSet MagicDiceSet { get; }
        public abstract Scriptable.DiceSet DefenseDiceSet { get; }
        public abstract int AttackDiceAmount { get; }
        public abstract int DefenseDiceAmount { get; }
        public bool Initialized { private set; get; }
        public event IToken.TokenEvent OnDestroyed;
        public event IToken.TokenEvent OnStatsChanged;
        public event IToken.TokenEvent OnActionsChanged;
        public event IToken.TokenEvent OnHealthChanged;
        public event IToken.TokenEvent OnManaChanged;
        public event IToken.TokenAttackEvent OnAttackPerformed;
        public int Speed => Scriptable.Speed + speedBonus;
        public AttackType AttackType => Scriptable.AttackType;
        public InteractableOutline InteractableOutline => interactableOutline;
        public Card TokenCard => Card;
        Scriptable.Token IToken.ScriptableToken => Scriptable;
        public Transform TokenTransform => transform;
        public int TokenActionPoints => ActionPoints;
        public void SetCard(Card card) => Card = card;
        public Ability[] Abilities => abilities.ToArray();
        public RangedAttackVisualizer RangedAttackVisualizer => rangedAttackVisualizer;
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
            bool hit = DiceUtil.CalculateAttackDiceThrow(AttackDiceAmount, AttackDiceSet, AttackPower,
                out int damage, out int[] attackSides);
            bool def = DiceUtil.CalculateDefenseDiceThrow(target.DefenseDiceAmount, target.DefenseDiceSet, target.Defense, 
                out int defensed, out int[] defenseSides);
            
            if (this is IUncontrollableToken)
            {
                await DiceManager.ThrowReplay(
                    target.DefenseDiceSet, target.DefenseDiceAmount, 
                    defenseSides.Concat(attackSides).ToArray(),
                    AttackDiceSet, AttackDiceAmount);
            } 
            else if (this is IControllableToken)
            {
                await DiceManager.ThrowReplay(
                    AttackDiceSet, AttackDiceAmount, 
                    attackSides.Concat(defenseSides).ToArray(),
                    target.DefenseDiceSet, target.DefenseDiceAmount);
            }
            
            Debug.Log($"Hit: {hit}, Damage: {damage}, Defensed: {defensed}");
            rangedAttackVisualizer.SetActive(false);
            if(!hit)
            {
                ((IToken) this).InvokeOnTokenMissGlobal();
                return;
            }
            
            int finalDamage = def ? Mathf.Clamp(damage - defensed, 0, int.MaxValue) : damage;
            target.Damage(finalDamage, aggroManager: aggroManager);
            OnAttackPerformed?.Invoke(this, target, Scriptable.AttackType, damage, defensed);
            await PlayAttackAnimation(target.TokenTransform);
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
            animationTween = null;
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