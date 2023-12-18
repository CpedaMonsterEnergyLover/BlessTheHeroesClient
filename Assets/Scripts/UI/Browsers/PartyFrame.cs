using Cysharp.Threading.Tasks;
using DG.Tweening;
using Effects;
using Gameplay.GameCycle;
using Gameplay.Tokens;
using Scriptable;
using UI.Elements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Browsers
{
    public class PartyFrame : PoolObject, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image deadImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private ProgressBar health;
        [SerializeField] private ProgressBar mana;
        [SerializeField] private ActionIndicator actions;
        [SerializeField] private Transform movePivot;
        [SerializeField] private Image bagButton;
        
        private Tween damageTween;
        private Tween sizeTween;
        private Tween moveTween;

        private IToken currentToken;
        
        public Inventory Inventory { get; set; }
        
        

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            AnimateResize(1f);
            OnTokenBrowserTokenSelected(TokenBrowser.SelectedToken);
        }

        public override void OnPool() => gameObject.SetActive(false);

        public override void OnTakenFromPool() => gameObject.SetActive(true);


        public void SetToken(IControllableToken token)
        {
            currentToken = token;
            icon.sprite = token.ScriptableToken.Sprite;
            UpdateHealth(token);
            UpdateMana(token);
            UpdateActions(token);
            UpdateSize(token);
            UpdateOutline(token);
            SubEvents(token);
        }

        private void UpdateHealth(IToken token)
        {
            health.UpdateValue(token.CurrentHealth, token.MaxHealth);
        }

        private void UpdateMana(IToken token)
        {
            mana.UpdateValue(token.CurrentMana, token.MaxMana);
            mana.SetActive(token.ScriptableToken.Mana != 0);
        }

        private void UpdateActions(IToken token)
        {
            actions.SetActions(token.ActionPoints);
        }

        private void UpdateSize(IControllableToken token)
        {
            ((RectTransform) transform).sizeDelta = Vector2.one * (token is HeroToken ? 110 : 85 );
        }

        private void UpdateDead(IToken token)
        {
            if(damageTween is not null) damageTween.Kill();
            deadImage.color = deadImage.color.WithAlpha(0.5f);
        }

        private void OnTokenDeath(IToken token)
        {
            if (token is CompanionToken)
            {
                UnsubEvents(token);
                Pool();
            } else UpdateDead(token);
        }

        private void OnTokenDestroy(IToken token)
        {
            UnsubEvents(token);
            AnimateResize(0);
            OnTokenDestroyAsync().Forget();
        }

        private async UniTask OnTokenDestroyAsync()
        {
            await UniTask.WaitUntil(() => sizeTween is null);
            Pool();
        }

        private void PlayDamage(DamageType damagetype, int damage)
        {
            if(damageTween is not null) damageTween.Kill();
            deadImage.color = deadImage.color.WithAlpha(0.82f);
            damageTween = deadImage
                .DOColor(deadImage.color.WithAlpha(0f), 1f)
                .OnKill(() => damageTween = null);
        }

        private void UpdateOutline(IToken token)
        {
            bool canInteract = token.CanInteract;
            if(canInteract) outlineImage.color = token.OutlineColor;
            outlineImage.enabled = canInteract;
        }

        private void OnMonstersTurnStarted() => outlineImage.enabled = false;

        private void OnTokenBrowserTokenSelected(IToken token)
        {
            AnimateMove(token == currentToken ? 75 : 50f);
        }
        
        private void SubEvents(IToken token)
        {
            TurnManager.OnMonstersTurnStarted += OnMonstersTurnStarted;
            TokenBrowser.OnTokenSelected += OnTokenBrowserTokenSelected;
            token.OnDamaged += PlayDamage;
            token.OnDeath += OnTokenDeath;
            token.OnDestroyed += OnTokenDestroy;
            token.OnHealthChanged += UpdateHealth;
            token.OnManaChanged += UpdateMana;
            token.OnActionsChanged += UpdateActions;
            token.OnActionsChanged += UpdateOutline;
            token.OnMovementPointsChanged += UpdateOutline;
            if (token is HeroToken hero) hero.OnResurrect += UpdateDead;
        }
        
        private void UnsubEvents(IToken token)
        {
            TurnManager.OnMonstersTurnStarted -= OnMonstersTurnStarted;
            TokenBrowser.OnTokenSelected -= OnTokenBrowserTokenSelected;
            token.OnDamaged -= PlayDamage;
            token.OnDeath -= OnTokenDeath;
            token.OnDestroyed -= OnTokenDestroy;
            token.OnHealthChanged -= UpdateHealth;
            token.OnManaChanged -= UpdateMana;
            token.OnActionsChanged -= UpdateActions;
            token.OnActionsChanged -= UpdateOutline;
            token.OnMovementPointsChanged -= UpdateOutline;
            if (token is HeroToken hero) hero.OnResurrect -= UpdateDead;
        }

        private void AnimateResize(float endvalue)
        {
            if(sizeTween is not null) sizeTween.Kill();
            sizeTween = transform.DOScale(endvalue, 0.25f)
                .OnKill(() => sizeTween = null);
        }
        
        private void AnimateMove(float endvalue)
        {
            if(moveTween is not null) moveTween.Kill();
            moveTween = movePivot.DOLocalMoveX(endvalue, 0.25f)
                .OnKill(() => moveTween = null);
        }

        
        // Pointer events
        public void OnPointerClick(PointerEventData eventData)
        {
            if(Input.GetMouseButtonUp(0))
            {
                TokenBrowser.SelectToken(currentToken);
                if (eventData.clickCount == 2)
                {
                    // TODO: camera
                }
            } 
        }

        public void ToggleInventory()
        {
            if(currentToken is HeroToken hero) Inventory.Toggle(hero, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(currentToken is not HeroToken) return;
            bagButton.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(currentToken is not HeroToken) return;
            bagButton.enabled = false;
        }
    }
}