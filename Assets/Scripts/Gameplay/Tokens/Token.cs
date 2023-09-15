using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens.Buffs;
using UI;
using UI.Elements;
using UnityEngine;
using Util;
using Util.Enums;
using Util.Interface;
using Util.Patterns;
using Util.Tokens;

namespace Gameplay.Tokens
{
    [RequireComponent(typeof(BuffManager))]
    public abstract partial class Token<T> : MonoBehaviour, 
        IInteractableOnClick, IToken, IHasHealth, IHasMana, IHasAnimation, IHasTokenDragEvent
        where T : Scriptable.Token
    {
        [Header("TokenBase Fields")]
        [SerializeField] protected RangedAttackVisualizer rangedAttackVisualizer;
        [SerializeField] private DamageAnimator damageAnimator;
        [SerializeField] private TokenOutline tokenOutline;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] private BuffManager buffManager;

        private readonly Ability[] abilities = new Ability[4];

        protected int maxHealthBonus;
        protected int maxManaBonus;
        protected int spellPower;
        protected int attackPower;
        protected int defense;
        protected int speedBonus;
        protected Tween animationTween;

        public abstract bool CanBeTargeted { get; }
        protected Card Card { get; set; }
        public T Scriptable { get; private set; }

        

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
            OnTokenDestroy?.Invoke(this);
        }


        // Class methods
        protected abstract void OnDeath();

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
            tokenOutline.SetOutlineWidth(GlobalDefinitions.TokenOutlineWidth);
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

        protected void UpdateOutlineByCanInteract() => tokenOutline.SetEnabled(Initialized && 
                                                                               this is IControllableToken { CanInteract: true });

        protected void InvokeDataChangedEvent() => OnTokenDataChanged?.Invoke(this);
        
        protected bool ConsumeActionPointForMovement()
        {
            if (ActionPoints <= 0) return false;
            ActionPoints--;
            MovementPoints += Speed;
            InvokeDataChangedEvent();
            return true;
        }

        private async UniTaskVoid HealAsync(int damage)
        {
            ((IHasHealth) this).SetHealth(Mathf.Clamp(CurrentHealth + damage, CurrentHealth, MaxHealth));
            await damageAnimator.PlayHealing(damage, 200);
        }
        
        private async UniTask DamageAsync(int damage, int delayMS, Sprite overrideDamageSprite)
        {
            int health = CurrentHealth - damage;
            
            if (health <= 0)
            {
                Dead = true;
                await damageAnimator.PlayDamage(damage, delayMS, overrideDamageSprite);
                ((IHasHealth) this).SetHealth(health);
                OnDeath();
                Despawn().Forget();
            }
            else
            {
                await damageAnimator.PlayDamage(damage, delayMS, overrideDamageSprite);
                ((IHasHealth) this).SetHealth(health);
                
                if(IToken.DraggedToken is not null) 
                    tokenOutline.SetEnabled(CanBeTargeted && 
                                            IToken.DraggedToken.TokenActionPoints != 0 && 
                                            IsInAttackRange(IToken.DraggedToken));
            }
        }

        public async UniTaskVoid Despawn()
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
            Card previous = Card;
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

       
        // IHasTokenDragEvent
        public virtual void OnTokenStartDraggingEventInvoked(IToken token) => tokenOutline.SetEnabled(false);

        public virtual void OnTokenEndDraggingEventInvoked(IToken token) => UpdateOutlineByCanInteract();


        // IToken
        public GameObject GameObject => gameObject;
        public bool Dead { get; private set; }
        public abstract Scriptable.DiceSet AttackDiceSet { get; }
        public abstract Scriptable.DiceSet MagicDiceSet { get; }
        public abstract Scriptable.DiceSet DefenseDiceSet { get; }
        public abstract int AttackDiceAmount { get; }
        public abstract int DefenseDiceAmount { get; }
        public bool Initialized { private set; get; }
        public event IToken.TokenEvent OnTokenDestroy;
        public event IToken.TokenEvent OnTokenDataChanged;
        public event IToken.TokenAttackEvent OnAttackPerformed;
        public int Speed => Scriptable.Speed + speedBonus;
        public AttackType AttackType => Scriptable.AttackType;
        public TokenOutline TokenOutline => tokenOutline;
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
            bool hit = DiceUtil.CalculateAttackDiceThrow(AttackDiceAmount, AttackDiceSet, 
                out int damage, out int[] attackSides);
            bool def = DiceUtil.CalculateDefenseDiceThrow(target.DefenseDiceAmount, target.DefenseDiceSet,
                out int defensed, out int[] defenseSides);
            int[] sides = def && target is HeroToken 
                ? defenseSides.Concat(attackSides).ToArray() 
                : attackSides.Concat(defenseSides).ToArray();
            await DiceManager.ThrowReplay(AttackDiceSet, AttackDiceAmount, sides, 
                against: target.DefenseDiceSet, againstAmount: target.DefenseDiceAmount);

            Debug.Log($"Hit: {hit}, Damage: {damage}, Defensed: {defensed}");
            if(!hit)
            {
                rangedAttackVisualizer.SetActive(false);
                return;
            }
            int finalDamage = def ? Mathf.Clamp(damage - defensed, 0, int.MaxValue) : damage;
            target.Damage(finalDamage);
            
            rangedAttackVisualizer.SetActive(false);
            OnAttackPerformed?.Invoke(this, target, this is HeroToken hero ? hero.Scriptable.AttackType : AttackType.Melee, damage, defensed);
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

        
        // IInteractableOnClick
        public abstract bool CanInteract { get; }
        public abstract bool CanClick { get; }
        public void OnClick(InteractionResult result)
        {
            TokenBrowser.Instance.SelectToken(this);
        }
        
        
        // IHasHealth
        public int CurrentHealth { get; set; }
        public int MaxHealth => Scriptable.Health + maxHealthBonus;
        
        
        // IHasMana
        public int CurrentMana { get; set; }
        public int MaxMana => Scriptable.Mana + maxManaBonus;
        
        
        // IHasAnimation
        public bool IsPlayingAnimation => 
            damageAnimator.IsPlayingAnimation || 
            (animationTween is not null && animationTween.IsPlaying());
    }
}