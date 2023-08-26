using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Dice;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Scriptable;
using UI;
using UI.Interaction;
using UnityEngine;
using Util.Enums;
using Util.Interaction;
using Util.Patterns;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public class HeroToken : Token<Hero>
    {
        public override bool CanInteract => 
            Controllable &&
            !Dead &&
            IToken.DraggedToken is null &&
            !IsPlayingAnimation &&
            (ActionPoints > 0 || MovementPoints > 0);

        public override bool CanClick => true;
        public override int Speed => Scriptable.Speed;
        public override int AttackDiceAmount => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackDiceAmount : 0;
        public override int DefenseDiceAmount => HasEquipmentInSlot(1) ? ((Armor) equipment[1]).DefenceDiceAmount : 0;
        public override DiceSet AttackDiceSet => DiceManager.AttackDiceSet;
        public override DiceSet MagicDiceSet => DiceManager.MagicDiceSet;
        public override DiceSet DefenseDiceSet => DiceManager.DefenceDiseSet;
        public override bool CanBeTargeted => false;

        private readonly Equipment[] equipment = new Equipment[4];



        // Class methods
        protected override void Init()
        {
            base.Init();
            Controllable = true;
            ActionPoints = 2;
            MovementPoints = Scriptable.Speed;
            for (var i = 0; i < 4; i++) 
                if(Scriptable.Equipment[i] is not null)
                    Equip(Scriptable.Equipment[i], i);
            // Resets HP and MP to max after their equipment changes
            ((IHasHealth) this).SetHealth(MaxHealth);
            ((IHasMana) this).SetMana(MaxMana);
            TokenBrowser.Instance.SelectFirst(this);
        }
        
        protected override void OnDeath()
        {
            Debug.Log("Hero is dead");
        }

        protected override InteractionTooltipData OnHoverCreature(CreatureToken creature)
        {
            if (!creature.CanBeTargeted) return new InteractionTooltipData();

            if (AttackDiceAmount == 0)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "Hero doesn't\nhave a weapon");
            }
            
            if (ActionPoints == 0)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "Not enough\nACT");
            }
            
            return creature.IsInAttackRange(this)
                ? new InteractionTooltipData(InteractionMode.Attack, 
                    InteractionState.Allow, "Attack")
                : new InteractionTooltipData(InteractionMode.Attack, 
                    InteractionState.Abandon, "Attack", "Target\nnot in range");
        }
        
        protected override InteractionTooltipData OnHoverCard(Card card)
        {
            if (card.IsPlayingAnimation) return new InteractionTooltipData();

            string title = "";
            string subTitle = "This card\nis too far";
            InteractionMode mode = InteractionMode.None;
            InteractionState state = InteractionState.None;
            bool inWalkRange = PatternSearch.CheckPlus(Card.GridPosition, card.GridPosition, 1, false);


            if (inWalkRange)
            {
                mode = card.IsOpened ? InteractionMode.Move : InteractionMode.OpenCard;
                title = card.IsOpened ? "Move" : "Open";
                state = InteractionState.Allow;
            }
            else if(Card == card)
            {
                if (card.HasAvailableAction)
                {
                    subTitle = "Not enough\nACT";
                    title = card.Scriptable.CardAction.Name;
                    mode = InteractionMode.Action;
                    state = ActionPoints > 0 ? InteractionState.Allow : InteractionState.Abandon;
                } 
            }
            else
            {
                mode = card.HasAvailableAction ? InteractionMode.Action : InteractionMode.OpenCard;
                state = InteractionState.Abandon;
            }
            
            
            return new InteractionTooltipData(mode: mode, state: state, actionTitle: title,  actionSubtitle: subTitle);
        }
                
        protected override InteractionTooltipData OnHoverOther()
        {
            return new InteractionTooltipData();
        }

        protected override void OnDragOnCreature(CreatureToken creature)
        {
            if (ActionPoints == 0 || 
                !creature.CanBeTargeted || 
                !creature.IsInAttackRange(this) ||
                AttackDiceAmount == 0)
            {
                rangedAttackVisualizer.SetActive(false);
                return;
            }
            
            Attack(creature).Forget();
        }

        protected override void OnDragOnCard(Card card)
        {
            if(card.IsPlayingAnimation) return;
            if (Card == card)
            {
                UseCardAction(card);
                return;
            }
            
            if(!PatternSearch.CheckNeighbours(Card.GridPosition, card.GridPosition)) 
                return;
            
            if (card.IsOpened)
                Walk(card);
            else
                OpenCard(card);
        }

        public override void OnDragStart(InteractionResult result)
        {
            base.OnDragStart(result);
            PatternSearch.IterateNeighbours(Card.GridPosition, pos =>
            {
                if(FieldManager.GetCard(pos, out Card card)) 
                    card.TokenOutline.SetEnabled(!card.IsPlayingHeroesAnimation);
            });
            if (ActionPoints != 0)
            {
                if (Card.HasAvailableAction)
                    Card.TokenOutline.SetEnabled(true);
                
                if(AttackDiceAmount != 0)
                    foreach (Card card in GetCardsInAttackRange()) 
                        card.OutlineAttackableCreatures(true);
            }
        }

        public override void OnDragEnd(InteractionResult result)
        {
            PatternSearch.IteratePlus(Card.GridPosition, 1,pos =>
            {
                if(FieldManager.GetCard(pos, out Card card)) card.TokenOutline.SetEnabled(false);
            });
            foreach (Card card in GetCardsInAttackRange()) 
                card.OutlineAttackableCreatures(false);
            base.OnDragEnd(result);
        }

        protected override void OnPlayersTurnStarted()
        {
            ActionPoints = 2;
            MovementPoints = Scriptable.Speed;
            UpdateOutlineByCanInteract();
            InvokeDataChangedEvent();
        }

        protected override void OnMonstersTurnStarted()
        {
            TokenOutline.SetEnabled(false);
        }
        


        private List<Card> GetCardsInAttackRange()
        {
            List<Card> cards = new();
            switch (Scriptable.AttackType)
            {
                case AttackType.Melee:
                    return new List<Card> { Card };
                /*case AttackType.Magic:
                    PatternSearch.IterateNeighbours(Card.GridPosition, pos => {
                        if(FieldManager.GetCard(pos, out Card card)) cards.Add(card);
                    });
                    cards.Add(Card);
                    return cards;*/
                case AttackType.Ranged:
                {
                    PatternSearch.IterateNeighbours(Card.GridPosition, pos => {
                        if(FieldManager.GetCard(pos, out Card card)) cards.Add(card);
                    });
                    return cards;
                }
                default:
                    return cards;
            }
        }

        private void OpenCard(Card card)
        {
            if (MovementPoints <= 0 && !ConsumeActionPointForMovement()) return;
            
            SetMovementPoints(MovementPoints - 1);
            card.Open();
        }

        private void UseCardAction(Card card)
        {
            if(ActionPoints == 0 || !card.HasAvailableAction) return;
            card.Scriptable.CardAction.Execute(card, this);
            SetActionPoints(ActionPoints - 1);
        }

        public void Equip(Equipment item, int slot)
        {
            if(!item.CanEquipInSlot(slot)) return;
            
            Unequip(slot);
            maxManaBonus += item.Mana;
            ((IHasMana) this).SetMana(CurrentMana);
            maxHealthBonus += item.Health;
            ((IHasHealth) this).SetHealth(CurrentHealth);
            attackPower += item.AttackPower;
            defense += item.Defense;
            spellPower += item.SpellPower;
            equipment[slot] = item;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        public void Unequip(int slot)
        {
            var item = equipment[slot];
            if (item is null) return;

            InventoryManager.Instance.AddItem(equipment[slot], 1);
            maxManaBonus -= item.Mana;
            ((IHasMana) this).SetMana(CurrentMana - item.Mana < 0 ? 1 : CurrentMana - item.Mana);
            maxHealthBonus -= item.Health;
            ((IHasHealth) this).SetHealth(CurrentHealth - item.Health < 0 ? 1 : CurrentHealth - item.Health);
            attackPower -= item.AttackPower;
            defense -= item.Defense;
            spellPower -= item.SpellPower;
            equipment[slot] = null;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        public void ReturnLostHealthAndMana(Equipment unequipped, Equipment equipped)
        {
            ((IHasHealth) this).SetHealth(CurrentHealth + Mathf.Min(unequipped.Health, equipped.Health));
            ((IHasMana) this).SetMana(CurrentMana + Mathf.Min(unequipped.Mana, equipped.Mana));
        }
        
        public bool HasEquipmentInSlot(int slot) => equipment[slot] is not null;
        public Equipment GetEquipmentAt(int index) => equipment[index];
    }
}