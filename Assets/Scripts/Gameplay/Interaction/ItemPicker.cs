using System.Collections.Generic;
using System.Linq;
using Gameplay.Inventory;
using Gameplay.Tokens;
using Scriptable;
using UI;
using UI.Browsers;
using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public class ItemPicker : MonoBehaviour
    {
        [SerializeField] private PickedItem pickedItem;
        
        public static bool IsPicked { get; private set; }

        public delegate void OutlineEvent();
        public static event OutlineEvent OnItemPickedUp;
        public static event OutlineEvent OnItemPickedDown;
        
        private static readonly List<IInteractable> TargetsCache = new();
        private static ItemDragMode DragMode { get; set; } = ItemDragMode.Trade;

        private enum ItemDragMode
        {
            Trade,
            Use
        }
        
        
        
        private void OnEnable()
        {
            InventorySlot.OnUsableHoverEnter += OnUsableHoverEnter;
            InventorySlot.OnUsableHoverExit += OnUsableHoverExit;
            InventorySlot.OnItemDragStart += OnItemDragStart;
            InventorySlot.OnItemDragEnd += OnItemDragEnd;
            InventorySlot.OnItemDrag += OnItemDrag;
        }
        
        private void OnDisable()
        {
            InventorySlot.OnUsableHoverEnter -= OnUsableHoverEnter;
            InventorySlot.OnUsableHoverExit -= OnUsableHoverExit;
            InventorySlot.OnItemDragStart -= OnItemDragStart;
            InventorySlot.OnItemDragEnd -= OnItemDragEnd;
            InventorySlot.OnItemDrag -= OnItemDrag;
        }

        private bool CanTrade(InventoryManager giver, InventoryManager receiver)
        {
            return giver.TryGetHero(out HeroToken hero1) &&
                   receiver.TryGetHero(out HeroToken hero2) &&
                   hero1.Card.Equals(hero2.Card);
        }

        private bool TryGetItemReceiver(out InventoryManager inventoryManager)
        {
            IItemReceiver itemReceiver = PartyFrame.FrameUnderCursor;
            if (itemReceiver is null)
            {
                var interactionResult = InteractionManager.GetInteractionResult();
                itemReceiver = interactionResult.IsValid && interactionResult.Target is HeroToken hero ? hero : null;
            }

            if (itemReceiver is not null)
            {
                inventoryManager = itemReceiver.InventoryManager;
                return true;
            }

            inventoryManager = null;
            return false;
        }
        
        private void OnItemDragStart(InventorySlot slot)
        {
            if(IsPicked) return;
            IsPicked = true;
            
            TargetsCache.Clear();
            if (slot.Item is Usable usable && Input.GetMouseButton(0) && TokenBrowser.SelectedToken.CanAct)
            {
                DragMode = ItemDragMode.Use;
                usable.GetUseTargets(TargetsCache);
                var interactionResult = InteractionManager.GetInteractionResult();
                TokenBrowser.SelectedToken.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
            }
            else
            {
                DragMode = ItemDragMode.Trade;
                pickedItem.PickItem(slot.Item);
                pickedItem.ShowState(null, null);
                slot.Item.GetTradeTargets(TargetsCache);
            }
            
            UpdateOutlinesOnItemPickedUp();
        }

        private void OnItemDrag(InventorySlot slot)
        {
            if(!IsPicked) return;
            IsPicked = true;

            var caster = TokenBrowser.SelectedToken;
            if (DragMode is ItemDragMode.Use && slot.Item is Usable && caster.CanAct)
            {
                var interactionResult = InteractionManager.GetInteractionResult();
                bool validTarget = TargetsCache.Contains(interactionResult.Target);
                caster.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
                InteractionState state = interactionResult.IsValid
                    ? validTarget
                        ? InteractionState.Allow
                        : InteractionState.Abandon
                    : InteractionState.None;
                caster.InteractionLine.SetInteractableColor(state);
                caster.InteractionLine.UpdatePosition(interactionResult.IntersectionPoint);
            }
            else if(DragMode is ItemDragMode.Trade)
            {
                InventoryManager giver = slot.InventoryManager;
                bool? state = TryGetItemReceiver(out InventoryManager receiver) && !giver.Equals(receiver)
                    ? CanTrade(giver, receiver)
                    : null;
                pickedItem.ShowState(state, receiver);
            }
        }

        private void OnItemDragEnd(InventorySlot slot)
        {
            if(!IsPicked) return;

            IsPicked = false;
            pickedItem.ShowState(null, null);
            pickedItem.PickItem(null);
            UpdateOutlinesOnItemPickedDown();
            
            if (DragMode is ItemDragMode.Use && slot.Item is Usable usable)
            {
                var interactionResult = InteractionManager.GetInteractionResult();
                var caster = TokenBrowser.SelectedToken;
                caster.InteractionLine.Disable();
                if(caster.CanAct) usable.Use(interactionResult.Target);
            }
            else if(DragMode is ItemDragMode.Trade)
            {
                InventoryManager giver = slot.InventoryManager;
                if (!TryGetItemReceiver(out InventoryManager receiver) ||
                    receiver.Equals(giver) ||
                    !CanTrade(giver, receiver)) return;

                receiver.AddItem(slot.Item, slot.Amount, out int left);
                slot.InventoryManager.RemoveItem(slot.Item, slot.Amount - left);
            }
        }

        private void OnUsableHoverEnter(Usable usable)
        {
        }

        private void OnUsableHoverExit(Usable usable)
        {
        }
        
        private static void UpdateOutlinesOnItemPickedUp()
        {
            OnItemPickedUp?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.EnableOutline();
        }
        
        private static void UpdateOutlinesOnItemPickedDown()
        {
            OnItemPickedDown?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.UpdateOutline();
        }
    }
}