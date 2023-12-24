using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Cards.TerrainEffects;
using Gameplay.Dice;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using Util;
using Util.Interaction;
using Util.Patterns;
using Util.Tokens;


namespace Gameplay.Cards
{
    public partial class Card : MonoBehaviour, IInteractableOnClick
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TokenLayout creaturesLayout;
        [SerializeField] private TokenLayout heroesLayout;
        [SerializeField] private TMP_Text eventText;
        [SerializeField] private InteractableOutline interactableOutline;
        [SerializeField] private TerrainManager terrainManager;

        private bool dead;
        private readonly List<IUncontrollableToken> creatures = new(8);
        private readonly List<IControllableToken> heroes = new(8);
        
        public delegate void TokenMoveEvent(IToken token);
        public event TokenMoveEvent OnTokenAdded;
        public event TokenMoveEvent OnTokenRemoved;

        public delegate void MovementCostEvent(List<int> costModificators);
        public event MovementCostEvent OnMovementCostCollected;
        
        public delegate void TableDoubleClickEvent(Vector3 position);
        public static event TableDoubleClickEvent OnDoubleClick;

        private static readonly List<int> MovementCostCollectionCache = new();

        public int MovementCost
        {
            get
            {
                MovementCostCollectionCache.Clear();
                OnMovementCostCollected?.Invoke(MovementCostCollectionCache);
                return MovementCostCollectionCache.Count == 0 ? 1 : MovementCostCollectionCache.Max();
            }
        }

#if UNITY_EDITOR
        public Scriptable.Token debug_TokenToSpawn;
#endif


        
        public bool OpenOnStart { get; set; }
        public bool IsOpened { get; private set; }
        public Scriptable.Location Scriptable { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        // TODO: .where(x => !x.Dead)
        public List<IUncontrollableToken> Creatures => creatures.ToList();
        public List<IControllableToken> Heroes => heroes.ToList();
        public int CreaturesAmount => creatures.Count;
        public int HeroesAmount => heroes.Count;
        public bool IsPlayingHeroesAnimation => heroesLayout.IsPlayingAnimation;
        public bool IsPlayingCreaturesAnimation => creaturesLayout.IsPlayingAnimation;
        public bool IsPlayingAnimation => 
            animationSequence is not null ||
            IsPlayingHeroesAnimation || 
            IsPlayingCreaturesAnimation;

        public bool HasBoss => creatures.FirstOrDefault(c => c is BossToken) is not null;
        public bool HasAvailableAction => Scriptable.HasAction;
        private Sequence animationSequence;

        

        // Unity methods
        private void OnDestroy()
        {
            dead = true;
            OnDestroyed?.Invoke(this);
        }

        private void Start()
        {
            IsOpened = OpenOnStart;
            transform.rotation = IsOpened 
                ? Quaternion.Euler(Vector3.zero)
                : Quaternion.Euler(new Vector3(0, 0, 180));

            PreInit().Forget();
        }


        
        // Class methods
        private async UniTask PreInit()
        {
            await UniTask.WaitUntil(() => Scriptable is not null);
#if UNITY_STANDALONE
            spriteRenderer.sprite = Scriptable.Sprite;
#endif
            OnInitialized?.Invoke(this);
        }
        
        public void Open()
        {
            if(IsOpened) return;
            
            IsOpened = true;
            FieldManager.OpenedCardsCounter++;
            OpenAsync().Forget();
        }

        private async UniTaskVoid OpenAsync()
        {
            if (FieldManager.TryOpenStoryCard(GridPosition)) await PreInit();

            CreateTerrainEffect();
            await PlayOpeningAnimation();
            UpdateEventText(false, Scriptable.HasAction);
            AddResourceDrops();
            await ExecuteOpeningEvent();
        }

        private void CreateTerrainEffect()
        {
            if(Scriptable.HasTerrainEffect) terrainManager.ApplyEffect(Scriptable.TerrainEffect, int.MaxValue);
        }
        
        private async Task PlayOpeningAnimation()
        {
            animationSequence = DOTween.Sequence()
                .Append(transform.DORotate(Vector3.zero, 0.75f))
                .Insert(0, transform.DOJump(transform.position, 1, 1, 0.75f));

            await animationSequence.AsyncWaitForKill();
            animationSequence = null;
        }

        public void Close()
        {
            if(!IsOpened) return;
            
            FieldManager.OpenedCardsCounter--;
            IsOpened = false;
            animationSequence = DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0, 0, 180), 0.75f))
                .Insert(0, transform.DOJump(transform.position, 1, 1, 0.75f))
                .OnKill(() => animationSequence = null);
        }

        private async UniTask ExecuteOpeningEvent()
        {
            if (!Scriptable.HasOpeningEvent) return;

            int roll = Random.Range(0, 6);
            await DiceManager.ThrowReplay(DiceManager.EventDiceSet, 1, new[] { roll });
            
            if (Scriptable.OpeningEvaluator.Evaluate(roll + 1, out int result, out CardAction action))
                action.Execute(this, null, result);
        }

        private void AddResourceDrops()
        {
            if(Scriptable.DropTable is null) return;
            var drops = Scriptable.DropTable.DropLoot();
            inventoryManager.AddCoins(Scriptable.DropTable.DropCoins());
            if (drops.Count != 0) AddItemDrops(drops);
        }

        public bool HasSpaceForHero() => HeroesAmount < 8;
        public bool HasSpaceForBoss() => !HasBoss;
        public bool HasSpaceForCreature() => CreaturesAmount < 8 && !HasBoss;
        
        public bool HasSpaceForToken(IToken token)
        {
            return token switch
            {
                IControllableToken => HasSpaceForHero(),
                BossToken => HasSpaceForBoss(),
                _ => HasSpaceForCreature()
            };
        }

        // TODO: find BUGZ bcz i removed check for having a space
        public void AddToken(IToken token, bool resetPosition = false, bool except = false, bool instantly = true)
        {
            if (token is IControllableToken controllable)
            {
                heroes.Add(controllable);
                heroesLayout.AttachToken(controllable, resetPosition, except, instantly);
            }
            else if(token is IUncontrollableToken uncontrollable)
            {
                if (uncontrollable is BossToken)
                    PushOrDespawnCreatures();

                creatures.Add(uncontrollable);
                creaturesLayout.AttachToken(token, resetPosition, except, instantly);
            }
            token.Card = this;
            
            OnTokenAdded?.Invoke(token);
            if (token is HeroToken) TryGiveItems().Forget();
        }

        public async UniTask AddTokenAsync(IToken token)
        {
            AddToken(token, resetPosition: true, instantly: false);
            await UniTask.WhenAll(
                UniTask.WaitUntil(() => !IsPlayingCreaturesAnimation), 
                UniTask.WaitUntil(() => token.Initialized));
        }

        public void RemoveToken(IToken token, bool instantly = true)
        {
            if (token is IControllableToken controllable)
            {
                if (heroes.Contains(controllable))
                {
                    heroes.Remove(controllable);
                    heroesLayout.UpdateLayout(instantly: instantly);
                }
            } else if (token is IUncontrollableToken uncontrollable)
            {
                if (creatures.Contains(uncontrollable))
                {
                    creatures.Remove(uncontrollable);
                    creaturesLayout.UpdateLayout(instantly: instantly);
                }
            }
            
            OnTokenRemoved?.Invoke(token);
        }

        public void PushOrDespawnCreatures()
        {
            var neighbours = new List<Card>();
            PatternSearch.IteratePlus(GridPosition, 1, v =>
            {
                if (FieldManager.GetCard(v, out Card n) && n.IsOpened) 
                    neighbours.Add(n);
            }, includeCenter: false);
            
            foreach (IUncontrollableToken creature in Creatures)
            {
                if(creature is BossToken) return;

                neighbours = neighbours.OrderBy(_ => Random.value).ToList();
                bool pushed = false;
                foreach (Card neighbour in neighbours)
                {
                    pushed = creature.Push(neighbour);
                    if(pushed)
                        break;
                }

                if(!pushed) creature.Despawn().Forget();
            }
        }
        
        public void RemoveTokenWithoutUpdate(IToken token)
        {
            if (token is IControllableToken controllable)
                heroes.Remove(controllable);
            else if (token is IUncontrollableToken uncontrollable)
                creatures.Remove(uncontrollable);
        }

        public async UniTask UpdateLayout(IToken token, bool instantly = false)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            if (token is IControllableToken)
                heroesLayout.UpdateLayout(instantly: instantly);
            else if (token is IUncontrollableToken) 
                creaturesLayout.UpdateLayout(instantly: instantly);
        }
        
        public void SetScriptable(Scriptable.Location location)
        {
            Scriptable = location;
#if UNITY_EDITOR
            spriteRenderer.sprite = Scriptable.Sprite;
#endif
        }

        public async UniTask SpawnHeroes(List<Scriptable.Hero> heroesToSpawn)
        {
            await UniTask.WaitUntil(() => IsOpened);

            foreach (Scriptable.Hero hero in heroesToSpawn)
            {
                if(!HasSpaceForHero()) return;
                
                IToken token = GlobalDefinitions.CreateHeroToken(hero);
                AddToken(token, resetPosition: true, instantly: false);
                await UniTask.WhenAll(
                    UniTask.WaitUntil(()=> !heroesLayout.IsPlayingAnimation), 
                    UniTask.WaitUntil(() => token.Initialized));
            }
        }

        public void SetGridPosition(Vector2Int pos) => GridPosition = pos;

        public Vector3 GetLastTokenPosition(IToken token)
        {
            return token switch
            {
                IControllableToken => heroesLayout.GetLastChildPosition(),
                BossToken => creaturesLayout.BossPosition,
                CreatureToken => creaturesLayout.GetLastChildPosition(),
                _ => Vector3.zero
            };
        }

        private void UpdateEventText(bool hasEvent, bool hasAction)
        {
            eventText.SetText($"{(hasEvent ? "?" : string.Empty)}{(hasAction ? "!" : string.Empty)}");
        }
        
        
        
        // IInteractableOnClick
        public Vector4 OutlineColor => GlobalDefinitions.TokenOutlineGreenColor;
        public InteractableOutline InteractableOutline => interactableOutline;
        public event IInteractable.InteractableEvent OnDestroyed;
        public event IInteractable.InteractableEvent OnInitialized;
        public bool CanClick => true;
        public bool Dead => dead;
        public bool CanInteract => false;
        public void OnClick(InteractionResult result, int clickCount)
        {
            if(clickCount > 1) OnDoubleClick?.Invoke(result.IntersectionPoint);
        }
    }
}