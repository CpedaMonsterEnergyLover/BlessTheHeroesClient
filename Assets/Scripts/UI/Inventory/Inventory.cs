using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Inventory;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InventorySlot inventorySlotPrefab;
        [SerializeField] private Transform slotsTransform;
        [SerializeField] private TMP_Text fullnessText;
        [SerializeField] private TMP_Text coinsText;

        private InventorySlot[] slots;
        private Tween tween;
        private bool IsActive => slotsTransform.gameObject.activeInHierarchy;


        public void UpdateCoinsText(int amount)
        {
            if(IsActive) coinsText.SetText($"Coins: {amount}");
        }

        public void CreateInventorySlots(int amount)
        {
            slots = new InventorySlot[amount];
            for (int i = 0; i < amount; i++) 
                slots[i] = Instantiate(inventorySlotPrefab, slotsTransform);
        }

        public void Toggle()
        {
            if(tween is not null) return;
            bool state = !slotsTransform.gameObject.activeInHierarchy;
            if(state)
            {
                slotsTransform.gameObject.SetActive(true);
                slotsTransform.localScale = Vector3.zero;
                UpdateCoinsText(InventoryManager.Instance.Coins);
            }
            tween = slotsTransform.DOScale(state ? Vector3.one : Vector3.zero, 0.15f).OnComplete(() =>
            {
                if(!state) slotsTransform.gameObject.SetActive(false);
                tween = null;
            });
        }

        public void UpdateInventory(List<Item> items)
        {
            if(!gameObject.activeInHierarchy) return;
            
            int slotIndex = 0;
            int slotsAmount = slots.Length;
            
            foreach (Item item in items)
            {
                int stacks = item.GetStacks(out int lastStack);
                int stackSize = item.Scriptable.StackSize;
                
                for (int s = 0; s < stacks; s++)
                {
                    if(slotIndex >= slotsAmount) break;
                    slots[slotIndex].SetItem(item, stackSize);
                    slotIndex++;
                }
                
                if (lastStack != 0)
                {
                    if(slotIndex >= slotsAmount) break;
                    slots[slotIndex].SetItem(item, lastStack);
                    slotIndex++;
                }
            }
            
            for (int i = slotIndex; i < slotsAmount; i++) 
                slots[i].SetItem(null, 0);
            
            fullnessText.SetText($"{slotIndex}/{slotsAmount}");
        }
    }
}