using System.Linq;
using Gameplay.Tokens;
using Gameplay.Tokens.Buffs;
using TMPro;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using Util.Enums;

namespace UI
{
    public class TokenBrowser : MonoBehaviour
    {
        [SerializeField] private Image portrait;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private ProgressBar manaBar;
        [SerializeField] private ProgressBar healthBar;
        [SerializeField] private EquipmentSlot[] equipmentSlots = new EquipmentSlot[4];
        [SerializeField] private AbilitySlot[] abilitySlots = new AbilitySlot[4];
        [SerializeField] private BuffIcon[] buffIcons = new BuffIcon[16];
        [SerializeField] private BuffIcon[] debuffIcons = new BuffIcon[16];
        
        // TODO: make Instance private and expose public methods
        public static TokenBrowser Instance { get; private set; }
        public IToken SelectedToken { get; private set; }
        private TokenBrowser() => Instance = this;



        public void SelectFirst(IToken token)
        {
            if(SelectedToken is not null) return;
            SelectToken(token);
        }
        
        public void SelectToken(IToken token)
        {
            if(SelectedToken == token) return;
            if(SelectedToken is not null) UnsubTokenEvents(SelectedToken);
            SelectedToken = token;
            
            portrait.sprite = token.ScriptableToken.Sprite;
            nameText.SetText(token.ScriptableToken.Name);
            manaBar.UpdateValues(token.CurrentMana, token.MaxMana);
            manaBar.SetActive(token.ScriptableToken.Mana != 0);
            healthBar.UpdateValues(token.CurrentHealth, token.MaxHealth);
            UpdateAbilities(token);
            UpdateBuffs(token);
            UpdateDebuffs(token);

            switch (token)
            {
                case IControllableToken controllable:
                    SelectControllable(controllable);
                    break;
                case IUncontrollableToken creature:
                    SelectCreature(creature);
                    break;
            }

            SubTokenEvents(token);
        }
        
        private void SelectControllable(IControllableToken controllable)
        {
            if (controllable is HeroToken hero)
                UpdateEquipment(hero);
            else 
                ClearEquipment();
            UpdateControllableStatsText(controllable);
        }

        private void SelectCreature(IUncontrollableToken creature)
        {
            ClearEquipment();
            UpdateCreatureStatsText(creature);
        }

        public void OnActionsChanged(IToken token)
        {
            if(ReferenceEquals(SelectedToken, token))
                foreach (AbilitySlot slot in abilitySlots) 
                    slot.UpdateInteractable(token);
        }

        public void OnManaChanged(int current, int max)
        {
            manaBar.UpdateValues(current, max);
            foreach (AbilitySlot slot in abilitySlots) 
                slot.OnManaChanged(SelectedToken);
        }

        public void OnHealthChanged(int current, int max)
        {
            healthBar.UpdateValues(current, max);
        }

        private void OnDataChanged(IToken token)
        {
            switch (token)
            {
                case IControllableToken controllable:
                    UpdateControllableStatsText(controllable);
                    break;
                case IUncontrollableToken creature:
                    UpdateCreatureStatsText(creature);
                    break;
            }
        }

        private void OnTokenDestroy(IToken token) => UnsubTokenEvents(token);
        
        public void UpdateEquipment(HeroToken heroToken)
        {
            if(!ReferenceEquals(heroToken, SelectedToken)) return;
            
            for (int i = 0; i < 4; i++)
            {
                var equip = heroToken.GetEquipmentAt(i);
                equipmentSlots[i].SetItem(equip);
            }
        }

        private void ClearEquipment()
        {
            for (var i = 0; i < equipmentSlots.Length; i++) 
                equipmentSlots[i].SetItem(null);
        }

        private void UpdateAbilities(IToken token)
        {
            var abilities = token.Abilities;
            for (int i = 0; i < 4; i++) 
                abilitySlots[i].SetAbility(abilities[i]);
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

        public void UpdateEffectsChanges(IToken token, BuffEffect effect)
        {
            if (effect.Scriptable.EffectType is BuffEffectType.Negative)
                UpdateDebuffs(token);
            else 
                UpdateBuffs(token);
        }
      
        private void UpdateControllableStatsText(IToken token)
        {
            var scriptable = token.ScriptableToken;
            statsText.SetText(
                $"{scriptable.AttackType}\n" +
                $"Spell+{token.SpellPower}\n" +
                $"Attack+{token.AttackPower}\n" +
                $"Defense+{token.Defense}\n" +
                $"SPD: {token.Speed}\n" +
                $"ACT: {token.ActionPoints}\n" +
                $"MOV:{token.MovementPoints}");
        }

        private void UpdateCreatureStatsText(IUncontrollableToken creature)
        {
            statsText.SetText($"ATK: {creature.AttackDiceAmount}\nDEF:{creature.DefenseDiceAmount}\nACT: {creature.ActionPoints}\nMOV: {creature.MovementPoints}");
        }

        private void SubTokenEvents(IToken token)
        {
            token.OnTokenDestroy += OnTokenDestroy;
            token.OnTokenDataChanged += OnDataChanged;
        }

        private void UnsubTokenEvents(IToken token)
        {
            token.OnTokenDestroy -= OnTokenDestroy;
            token.OnTokenDataChanged -= OnDataChanged;
        }
    }
}