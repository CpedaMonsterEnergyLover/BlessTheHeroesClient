using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using UI;
using UI.Elements;
using UI.Interaction;
using UnityEngine;
using Util;
using Util.Enums;
using Util.Interface;
using Util.Patterns;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public abstract partial class Token<T> : MonoBehaviour, 
        IInteractableOnDrag, IInteractableOnClick, IToken, IHasHealth, IHasMana, IHasAnimation, IHasTokenDragEvent
        where T : Scriptable.Token
    {
        [Header("TokenBase Fields")]
        [SerializeField] protected RangedAttackVisualizer rangedAttackVisualizer;
        [SerializeField] private DamageAnimator damageAnimator;
        [SerializeField] private TokenOutline tokenOutline;
        [SerializeField] private InteractionLine interactionLine;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        private readonly Ability[] abilities = new Ability[4];

        protected int maxHealthBonus;
        protected int maxManaBonus;
        protected int spellPower;
        protected int attackPower;
        protected int defense;
        protected Sequence animationTween;

        public abstract bool CanBeTargeted { get; }
        public bool Dead { get; private set; }
        protected Card Card { get; private set; }
        public T Scriptable { get; private set; }
        public bool Controllable { get; protected set; }
        public int ActionPoints { get; protected set; }
        public int MovementPoints { get; protected set; }
        public int SpellPower => spellPower;
        public int AttackPower => attackPower;
        public int Defense => defense;

        

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
        protected abstract InteractionTooltipData OnHoverCreature(CreatureToken creature);
        protected abstract InteractionTooltipData OnHoverCard(Card card);
        protected abstract InteractionTooltipData OnHoverOther();
        protected abstract void OnDragOnCreature(CreatureToken creature);
        protected abstract void OnDragOnCard(Card card);
        protected abstract void OnPlayersTurnStarted();
        protected abstract void OnMonstersTurnStarted();

        
        
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

        protected void UpdateOutlineByCanInteract() => tokenOutline.SetEnabled(Initialized && CanInteract);

        protected void InvokeDataChangedEvent() => OnTokenDataChanged?.Invoke(this);
        
        protected bool ConsumeActionPointForMovement()
        {
            if (ActionPoints <= 0) return false;
            ActionPoints--;
            MovementPoints += Speed;
            InvokeDataChangedEvent();
            return true;
        }

        private async UniTask DamageAsync(int damage, int delayMS)
        {
            ((IHasHealth) this).SetHealth(CurrentHealth -= damage);
            if (CurrentHealth <= 0)
            {
                Dead = true;
                Card.RemoveTokenWithoutUpdate(this);
                OnDeath();
            }
            await damageAnimator.PlayAsync(damage, delayMS);
            if (CurrentHealth <= 0)
            {
                Destroy(gameObject);
                // For UpdateLayout we need to wait until object has been destroyed
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                Card.UpdateLayoutWithoutRemoval(this);
            } 
            else if(IToken.DraggedToken is not null) 
                tokenOutline.SetEnabled(CanBeTargeted && IToken.DraggedToken.TokenActionPoints != 0 && IsInAttackRange(IToken.DraggedToken));
        }

        protected async UniTask Attack(IToken token)
        {
            if(AttackDiceAmount == 0) return;
            
            SetActionPoints(ActionPoints - 1);
            
            bool hit = DiceUtil.CalculateAttackDiceThrow(AttackDiceAmount, AttackDiceSet, 
                out int damage, out int[] attackSides);
            bool def = DiceUtil.CalculateDefenseDiceThrow(token.DefenseDiceAmount, token.DefenseDiceSet,
                out int defensed, out int[] defenseSides);
            int[] sides = def && token is HeroToken 
                ? defenseSides.Concat(attackSides).ToArray() 
                : attackSides.Concat(defenseSides).ToArray();
            await DiceManager.ThrowReplay(AttackDiceSet, AttackDiceAmount, sides, 
                against: token.DefenseDiceSet, againstAmount: token.DefenseDiceAmount);

            Debug.Log($"Hit: {hit}, Damage: {damage}, Defensed: {defensed}");
            if(!hit)
            {
                rangedAttackVisualizer.SetActive(false);
                return;
            }
            int finalDamage = def ? Mathf.Clamp(damage - defensed, 0, int.MaxValue) : damage;
            token.Damage(finalDamage);
            
            rangedAttackVisualizer.SetActive(false);
            OnAttackPerformed?.Invoke(this, token, this is HeroToken hero ? hero.Scriptable.AttackType : AttackType.Melee, damage, defensed);
            await PlayAttackAnimation(token.TokenTransform);
            UpdateOutlineByCanInteract();
        }

        protected void Walk(Card card)
        {
            if (MovementPoints <= 0 && !ConsumeActionPointForMovement()) return;
            
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
                    inst.SetToken(this);
                    abilities[i] = inst;
                }
            }
        }

        private async UniTask PlayAttackAnimation(Transform target)
        {
            if (this is HeroToken hero && hero.Scriptable.AttackType == AttackType.Ranged)
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

       
        // IHasTokenDragEvent
        public virtual void OnTokenStartDraggingEventInvoked(IToken token)
        {
            tokenOutline.SetEnabled(false);
        }

        public virtual void OnTokenEndDraggingEventInvoked(IToken token)
        {
            UpdateOutlineByCanInteract();
        }


        // IToken
        public bool Initialized { private set; get; }
        public abstract Scriptable.DiceSet AttackDiceSet { get; }
        public abstract Scriptable.DiceSet MagicDiceSet { get; }
        public abstract Scriptable.DiceSet DefenseDiceSet { get; }
        public abstract int Speed { get; }
        public abstract int AttackDiceAmount { get; }
        public abstract int DefenseDiceAmount { get; }
        public TokenOutline TokenOutline => tokenOutline;
        public Card TokenCard => Card;
        public event IToken.TokenEvent OnTokenDestroy;
        public event IToken.TokenEvent OnTokenDataChanged;
        public event IToken.TokenAttackEvent OnAttackPerformed;
        Scriptable.Token IToken.ScriptableToken => Scriptable;
        public Transform TokenTransform => transform;
        public int TokenActionPoints => ActionPoints;
        public InteractionLine InteractionLine => interactionLine;
        public void SetCard(Card card) => Card = card;
        public void Damage(int damage, int delayMS = 200) => DamageAsync(damage, delayMS).Forget();
        public Ability[] Abilities => abilities.ToArray();
        public RangedAttackVisualizer RangedAttackVisualizer => rangedAttackVisualizer;



        // IInteractableOnClick
        public abstract bool CanClick { get; }
        public void OnClick(InteractionResult result)
        {
            TokenBrowser.Instance.SelectToken(this);
        }

        
        // IInteractableOnDrag
        public abstract bool CanInteract { get; }
        public virtual void OnDragStart(InteractionResult result)
        {
            ((IToken) this).InvokeStartDraggingEvent();
            interactionLine.Enable(result.IntersectionPoint);
            TokenBrowser.Instance.SelectToken(this);
        }

        public InteractionTooltipData OnDrag(InteractionResult result)
        {
            bool valid = result.IsValid;
            interactionLine.SetEnabled(valid, result.IntersectionPoint);
            if(!valid)
            {
                rangedAttackVisualizer.SetActive(false);
                return null;
            }

            InteractionTooltipData tooltipData;
            switch (result.Target)
            {
                case CreatureToken creature:
                    rangedAttackVisualizer.SetActive(this is HeroToken hero &&
                                                     hero.Scriptable.AttackType is AttackType.Ranged,
                        result.IntersectionPoint);
                    tooltipData = OnHoverCreature(creature);
                    break;
                case Card card:
                    rangedAttackVisualizer.SetActive(false);
                    tooltipData = OnHoverCard(card);
                    break;
                default:
                    rangedAttackVisualizer.SetActive(false);
                    tooltipData = OnHoverOther();
                    break;
            }
            
            interactionLine.UpdatePosition(result.IntersectionPoint);
            interactionLine.SetInteractableColor(tooltipData.State);
            return tooltipData;
        }

        public virtual void OnDragEnd(InteractionResult result)
        {
            interactionLine.Disable();
            if (!result.IsValid)
            {
                ((IToken) this).InvokeOnEndDraggingEvent();
                return;
            }
            
            switch (result.Target)
            {
                case CreatureToken creature:
                    OnDragOnCreature(creature);
                    break;
                case Card card:
                    OnDragOnCard(card);
                    break;
            }
            
            ((IToken) this).InvokeOnEndDraggingEvent();
        }

        public bool IsInAttackRange(IToken attacker)
        {
            switch (attacker)
            {
                case CreatureToken creature:
                    return creature.Card == Card;
                case HeroToken hero:
                    AttackType attackType = hero.Scriptable.AttackType;
                    return attackType switch
                    {
                        AttackType.Melee => hero.Card == Card,
                        AttackType.Ranged => PatternSearch.CheckNeighbours(hero.TokenCard.GridPosition,
                            Card.GridPosition),
                        // AttackType.Magic => PatternSearch.CheckPlus(hero.TokenCard.GridPosition, Card.GridPosition, 1),
                        _ => false
                    };
                default:
                    return false;
            }
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