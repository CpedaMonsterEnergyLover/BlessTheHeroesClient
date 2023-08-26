using System.Collections.Generic;
using System.Linq;
using CardAPI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using Util;
using Util.Patterns;
using Util.Tokens;


namespace Gameplay.GameField
{
    public class Card : MonoBehaviour, 
        IInteractableOnClick
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TokenLayout creaturesLayout;
        [SerializeField] private TokenLayout heroesLayout;
        [SerializeField] private TokenOutline tokenOutline;
        [SerializeField] private TMP_Text eventText;
        
        private readonly List<CreatureToken> creatures = new(8);
        private readonly List<HeroToken> heroes = new(4);


        public bool OpenOnStart { get; set; }
        public bool IsOpened { get; private set; }
        public Scriptable.Location Scriptable { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public TokenOutline TokenOutline => tokenOutline;
        public List<CreatureToken> Creatures => creatures.ToList();
        public bool FullOfCreatures => creatures.Count == 8;
        public List<HeroToken> Heroes => heroes.ToList();
        public bool HasHeroes => heroes.Count > 0;
        public bool IsPlayingHeroesAnimation => heroesLayout.IsPlayingAnimation;
        public bool IsPlayingCreaturesAnimation => creaturesLayout.IsPlayingAnimation;
        public bool IsPlayingAnimation => 
            animationSequence is not null ||
            IsPlayingHeroesAnimation || 
            IsPlayingCreaturesAnimation;
        public bool HasAvailableAction => Scriptable.CardAction is not null;
        private Sequence animationSequence;

        

        // Unity methods
        private void Start()
        {
            IsOpened = OpenOnStart;
            transform.rotation = IsOpened 
                ? Quaternion.Euler(Vector3.zero)
                : Quaternion.Euler(new Vector3(0, 0, 180));
            tokenOutline.SetOutlineWidth(GlobalDefinitions.CardOutlineWidth);

            PreInit().Forget();
        }


        // Class methods
        private async UniTask PreInit()
        {
            await UniTask.WaitUntil(() => Scriptable is not null);
#if UNITY_STANDALONE
            spriteRenderer.sprite = Scriptable.Sprite;
#endif
            UpdateEventText(false, Scriptable.CardAction is not null);
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
            if (FieldManager.TryOpenStoryCard(GridPosition)) 
                await PreInit();
            
            animationSequence = DOTween.Sequence()
                .Append(transform.DORotate(Vector3.zero, 0.75f))
                .Insert(0, transform.DOJump(transform.position, 1, 1, 0.75f));

            await animationSequence.AsyncWaitForKill();
            DropResources();
            await ExecuteOpeningEvents();
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

        private async UniTask ExecuteOpeningEvents()
        {
            if (Scriptable.EvaluatorSet is null) return;

            int roll = Random.Range(0, 6);
            await DiceManager.ThrowReplay(DiceManager.EventDiceSet, 1, new[] { roll });
            
            if (Scriptable.EvaluatorSet.Evaluate(roll + 1, out int result, out CardAction action))
                action.Execute(this, null, result);
        }

        private void DropResources()
        {
            if(Scriptable.DropTable is null) return;
            var drop = Scriptable.DropTable.Drop;
            InventoryManager.Instance.AddCoins(Scriptable.DropTable.Coins);
            foreach (Scriptable.Item item in drop) 
                InventoryManager.Instance.AddItem(item, 1);
        }

        public void AddToken(IToken token, bool resetPosition = false, bool except = false, bool instantly = true)
        {
            switch (token)
            {
                case CreatureToken creature:
                    if (creatures.Count >= 8) return;
                    creatures.Add(creature);
                    creaturesLayout.AttachToken(creature, resetPosition, except, instantly);
                    break;
                case HeroToken hero:
                    if (heroes.Count >= 4) return;
                    heroes.Add(hero);
                    heroesLayout.AttachToken(hero, resetPosition, except, instantly);
                    break;
            }

            token.SetCard(this);
        }

        public void RemoveToken(IToken token, bool instantly = true)
        {
            switch (token)
            {
                case CreatureToken creature:
                    if (creatures.Contains(creature))
                    {
                        creatures.Remove(creature);
                        creaturesLayout.UpdateLayout(instantly: instantly);
                    }
                    break;
                case HeroToken hero:
                    if (heroes.Contains(hero))
                    {
                        heroes.Remove(hero);
                        heroesLayout.UpdateLayout(instantly: instantly);
                    }
                    break;
            }
        }

        public void RemoveTokenWithoutUpdate(IToken token)
        {
            switch (token)
            {
                case CreatureToken creature:
                    creatures.Remove(creature);
                    break;
                case HeroToken hero:
                    heroes.Remove(hero);
                    break;
            }
        }

        public void UpdateLayoutWithoutRemoval(IToken token, bool instantly = false)
        {
            switch (token)
            {
                case CreatureToken:
                    creaturesLayout.UpdateLayout(instantly: instantly);
                    break;
                case HeroToken:
                    heroesLayout.UpdateLayout(instantly: instantly);
                    break;
            }
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
                CreatureToken => creaturesLayout.GetLastChildPosition(),
                HeroToken => heroesLayout.GetLastChildPosition(),
                _ => Vector3.zero
            };
        }

        public void OutlineAttackableCreatures(bool isEnabled)
        {
            foreach (CreatureToken token in creatures) 
                token.TokenOutline.SetEnabled(isEnabled && token.CanBeTargeted);
        }

        public void OnTokenLayoutAnimationEnded(TokenLayout tokenLayout)
        {
            if (tokenLayout == heroesLayout)
            {
                if (IToken.DraggedToken is not null &&
                    PatternSearch.CheckNeighbours(IToken.DraggedToken.TokenCard.GridPosition, GridPosition))
                    tokenOutline.SetEnabled(true);
            }
        }

        private void UpdateEventText(bool hasEvent, bool hasAction)
        {
            eventText.SetText($"{(hasEvent ? "?" : string.Empty)}{(hasAction ? "!" : string.Empty)}");
        }
        
        
        
        // IInteractableOnClick
        public bool CanClick => true;
        public bool CanInteract => !IsOpened;
        public void OnClick(InteractionResult result) => Open();
    }
}