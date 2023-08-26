using Gameplay.Tokens;
using TMPro;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;

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

            switch (token)
            {
                case HeroToken hero:
                    SelectHero(hero);
                    break;
                case CreatureToken creature:
                    SelectCreature(creature);
                    break;
            }

            SubTokenEvents(token);
        }
        
        private void SelectHero(HeroToken hero)
        {
            UpdateEquipment(hero);
            UpdateHeroStatsText(hero);
        }

        private void SelectCreature(CreatureToken creature)
        {
            ClearEquipment();
            UpdateCreatureStatsText(creature);
        }

        public void OnActionChanged(HeroToken hero)
        {
            foreach (AbilitySlot slot in abilitySlots) 
                slot.UpdateInteractable(hero);
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
                case HeroToken hero:
                    UpdateHeroStatsText(hero);
                    break;
                case CreatureToken creature:
                    UpdateCreatureStatsText(creature);
                    break;
            }
        }

        private void OnTokenDestroy(IToken token) => UnsubTokenEvents(token);
        
        public void UpdateEquipment(HeroToken heroToken)
        {
#pragma warning disable CS0252, CS0253
            if(heroToken != SelectedToken) return;
#pragma warning restore CS0252, CS0253
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
      
        private void UpdateHeroStatsText(HeroToken hero)
        {
            Scriptable.Hero scriptable = hero.Scriptable;
            statsText.SetText(
                $"{scriptable.AttackType}\n" +
                $"Spell+{hero.SpellPower}\n" +
                $"Attack+{hero.AttackPower}\n" +
                $"Defense+{hero.Defense}\n" +
                $"SPD: {scriptable.Speed}\n" +
                $"ACT: {hero.ActionPoints}\n" +
                $"MOV:{hero.MovementPoints}");
        }

        private void UpdateCreatureStatsText(CreatureToken creature)
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