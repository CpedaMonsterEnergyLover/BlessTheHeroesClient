using System.Collections.Generic;
using System.Linq;
using Gameplay.Tokens;
using MyBox;
using Scriptable;
using TMPro;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using Util.Enums;
using BuffEffect = Gameplay.BuffEffects.BuffEffect;

namespace UI.Browsers
{
    public class TokenBrowser : MonoBehaviour
    {
        [Separator("Texts and Indicators")]
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text movementPointsText;
        [SerializeField] private ProgressBar manaBar;
        [SerializeField] private ProgressBar healthBar;
        [SerializeField] private StatsIndicator statsIndicator;
        [SerializeField] private ActionIndicator actionIndicator;
        [SerializeField] private AttackTypeIndicator attackTypeIndicator;
        [SerializeField] private ArmorTypeIndicator armorTypeIndicator;
        [SerializeField] private CreatureTypeIndicator creatureTypeIndicator;
        [SerializeField] private GameObject energyGO;
        [Separator("Slots")]
        [SerializeField] private EquipmentSlot[] equipmentSlots = new EquipmentSlot[4];
        [SerializeField] private Transform abilitiesTransform;
        [SerializeField] private AbilitySlot abilitySlotPrefab;
        [SerializeField] private BuffIcon[] buffIcons = new BuffIcon[16];
        [SerializeField] private BuffIcon[] debuffIcons = new BuffIcon[16];
        [SerializeField] private GameObject bagSlot;
        [SerializeField] private Inventory inventory;

        public delegate void TokenSelectedEvent(IToken token);
        public static event TokenSelectedEvent OnTokenSelected;
        
        private static TokenBrowser Instance { get; set; }
        private readonly List<AbilitySlot> abilitySlots = new();
        public static IToken SelectedToken { get; private set; }




        
        private void Awake()
        {
            Instance = this;
        }

        public static void SelectToken(IToken token)
        {
            Instance.Select(token);
        }
        
        private void Select(IToken token)
        {
            if(SelectedToken == token) return;
            if(SelectedToken is not null) UnsubTokenEvents(SelectedToken);
            SelectedToken = token;
            OnTokenSelected?.Invoke(token);
            
            portrait.sprite = token.ScriptableToken.Sprite;
            nameText.SetText(token.ScriptableToken.Name);
            manaBar.UpdateValue(token.CurrentMana, token.MaxMana);
            manaBar.SetActive(token.ScriptableToken.Mana != 0);
            healthBar.UpdateValue(token.CurrentHealth, token.MaxHealth);
            armorTypeIndicator.SetArmorType(token.ArmorType);
            attackTypeIndicator.SetAttackType(token.AttackType);
            statsIndicator.SetStats(token.Speed, token.SpellPower, token.AttackPower, token.Defense);
            movementPointsText.SetText(token.MovementPoints.ToString());
            actionIndicator.SetActions(token.ActionPoints);
            creatureTypeIndicator.SetCreatureType(token.ScriptableToken.CreatureType);

            UpdateAbilities(token);
            UpdateBuffs(token);
            UpdateDebuffs(token);
            

            if(token is HeroToken hero)
            {
                UpdateEquipment(hero);
                energyGO.SetActive(true);
                bagSlot.SetActive(true);
            } else {
                HideEquipment();
                energyGO.SetActive(false);
                bagSlot.SetActive(false);
            }

            SubTokenEvents(token);
        }

        private void OnActionsChanged(IToken token)
        {
            actionIndicator.SetActions(token.ActionPoints);
            foreach (var slot in abilitySlots.Where(slot => slot.isActiveAndEnabled)) 
                slot.UpdateInteractable(token);
        }

        private void OnManaChanged(IToken token)
        {
            manaBar.UpdateValue(token.CurrentMana, token.MaxMana);
            foreach (var slot in abilitySlots.Where(slot => slot.isActiveAndEnabled)) 
                slot.OnManaChanged(SelectedToken);
        }

        private void OnHealthChanged(IToken token)
        {
            healthBar.UpdateValue(token.CurrentHealth, token.MaxHealth);
        }

        private void OnStatsChanged(IToken token)
        {
            statsIndicator.SetStats(token.Speed, token.SpellPower, token.AttackPower, token.Defense);
        }

        private void OnMovementPointsChanged(IToken token)
        {
            movementPointsText.SetText(token.MovementPoints.ToString());
        }

        private void UpdateEquipment(HeroToken heroToken)
        {
            var abilities = heroToken.EquipmentAbilities;
            for (int i = 0; i < 4; i++)
            {
                var equip = heroToken.GetEquipmentAt(i);
                EquipmentSlot slot = equipmentSlots[i];
                if(equip is null) slot.ClearItem();
                else slot.SetItem(equip, abilities[i]);
                slot.gameObject.SetActive(true);
            }
        }

        private void HideEquipment()
        {
            foreach (var slot in equipmentSlots) slot.gameObject.SetActive(false);
        }

        private void UpdateAbilities(IToken token)
        {
            var abilities = token.Abilities;
            int requestedCount = abilities.Length;
            CreateMissingSlots(requestedCount - abilitySlots.Count);
            for (int i = 0; i < abilitySlots.Count; i++)
            {
                AbilitySlot slot = abilitySlots[i];
                if (i < requestedCount)
                {
                    slot.SetAbility(abilities[i]);
                    slot.gameObject.SetActive(true);
                }
                else slot.gameObject.SetActive(false);
            }
        }

        private void CreateMissingSlots(int amount)
        {
            if(amount <= 0) return;
            for(int i = 0; i < amount; i++)
                abilitySlots.Add(Instantiate(abilitySlotPrefab, abilitiesTransform));
        }

        public void ToggleInventory()
        {
            if(SelectedToken is HeroToken hero) inventory.Toggle(hero);
        }

        private void UpdateBuffs(IToken token)
        {
            var buffs = token.BuffManager.ActiveBuffs.Take(16).OrderBy(e => e.Scriptable.name).ToArray();
            int buffsAmount = buffs.Length;
            for (int i = 0; i < buffsAmount; i++) buffIcons[i].SetBuff(buffs[i]);
            for(int i = buffsAmount; i < 16; i++) buffIcons[i].SetBuff(null);
        }

        private void UpdateDebuffs(IToken token)
        {
            var buffs = token.BuffManager.ActiveDebuffs.Take(16).OrderBy(e => e.Scriptable.name).ToArray();
            int buffsAmount = buffs.Length;
            for (int i = 0; i < buffsAmount; i++) debuffIcons[i].SetBuff(buffs[i]);
            for(int i = buffsAmount; i < 16; i++) debuffIcons[i].SetBuff(null);
        }

        private void OnEffectApplied(IToken token, BuffEffect effect)
        {
            if (effect.Scriptable.EffectDirection is BuffEffectDirection.Negative)
                UpdateDebuffs(token);
            else 
                UpdateBuffs(token);
        }

        private void OnEquipped(HeroToken hero, int slot, Equipment item)
        {
            equipmentSlots[slot].SetItem(item, hero.EquipmentAbilities[slot]);
        }
        private void OnUnequipped(HeroToken hero, int slot, Equipment item)
        {
            equipmentSlots[slot].ClearItem();
        }
        
        private void OnDestroyed(IToken token) => UnsubTokenEvents(token);

        private void SubTokenEvents(IToken token)
        {
            if (token is IHeroToken hero)
            {
                hero.OnEquipped += OnEquipped;
                hero.OnUnequipped += OnUnequipped;
            }
            token.BuffManager.OnEffectApplied += OnEffectApplied;
            token.OnActionsChanged += OnActionsChanged;
            token.OnMovementPointsChanged += OnMovementPointsChanged;
            token.OnManaChanged += OnManaChanged;
            token.OnHealthChanged += OnHealthChanged;
            token.OnDestroyed += OnDestroyed;
            token.OnStatsChanged += OnStatsChanged;
        }

        private void UnsubTokenEvents(IToken token)
        {
            if (token is IHeroToken hero)
            {
                hero.OnEquipped -= OnEquipped;
                hero.OnUnequipped -= OnUnequipped;
            }
            token.BuffManager.OnEffectApplied -= OnEffectApplied;
            token.OnActionsChanged -= OnActionsChanged;
            token.OnMovementPointsChanged -= OnMovementPointsChanged;
            token.OnManaChanged -= OnManaChanged;
            token.OnHealthChanged -= OnHealthChanged;
            token.OnDestroyed -= OnDestroyed;
            token.OnStatsChanged -= OnStatsChanged;
        }
    }
}