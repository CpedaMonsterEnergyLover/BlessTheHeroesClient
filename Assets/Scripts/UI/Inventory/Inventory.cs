using DG.Tweening;
using Gameplay.Inventory;
using Gameplay.Tokens;
using TMPro;
using UI.Browsers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InventorySlot inventorySlotPrefab;
        [SerializeField] private Transform slotsTransform;
        [SerializeField] private TMP_Text fullnessText;
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private Image avatarImage;
        
        private InventorySlot[] slots;
        private Tween tween;
        private InventoryManager openedManager;

        public bool IsOpened { get; private set; }



        public void Toggle(HeroToken hero)
        {
            if(tween is not null) return;
            bool state = !slotsTransform.gameObject.activeInHierarchy;
            if (state)
                Open(hero);
            else 
                Close();
            PlayAnimation(state);
        }

        private void Open(HeroToken hero)
        {
            if(IsOpened) return;
            
            InventoryManager manager = hero.InventoryManager;
            openedManager = manager;
            IsOpened = true;
            LinkSlots();
            openedManager.OnCoinsUpdate += UpdateCoinsText;
            openedManager.OnItemsUpdate += UpdateInventory;
            UpdateHeroIcon(hero);
            UpdateCoinsText(openedManager.Coins);
            UpdateInventory(openedManager.Items);
            TokenBrowser.OnTokenSelected += SwitchToken;
        }

        public void Close()
        {
            if(!IsOpened) return;
            
            IsOpened = false;
            openedManager.OnCoinsUpdate -= UpdateCoinsText;
            openedManager.OnItemsUpdate -= UpdateInventory;
            TokenBrowser.OnTokenSelected -= SwitchToken;
            openedManager = null;
        }

        public void PlayAnimation(bool state)
        {
            if (state)
            {
                slotsTransform.localScale = Vector3.zero;
                slotsTransform.gameObject.SetActive(true);
            }
            tween = slotsTransform.DOScale(state ? Vector3.one : Vector3.zero, 0.15f).OnComplete(() =>
            {
                if(!state) slotsTransform.gameObject.SetActive(false);
                tween = null;
            });
        }

        private void UpdateInventory(Item[] items)
        {
            if(!IsOpened) return;
            
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

        private void UpdateHeroIcon(HeroToken hero) => avatarImage.sprite = hero.Scriptable.Sprite;

        private void UpdateCoinsText(int coins) => coinsText.SetText($"Coins: {coins}");

        private void LinkSlots()
        {
            if(slots is not null)
            {
                foreach (InventorySlot slot in slots) 
                    slot.InventoryManager = openedManager;
                return;
            }
            
            int amount = openedManager.Capacity;
            slots = new InventorySlot[amount];
            for (int i = 0; i < amount; i++)
            {
                var slot = Instantiate(inventorySlotPrefab, slotsTransform);
                slot.InventoryManager = openedManager;
                slots[i] = slot;
            }
        }

        private void SwitchToken(IToken token)
        {
            if(token is not HeroToken hero)
            {
                Close();
                PlayAnimation(false);
                return;
            }
            
            Close();
            Open(hero);
        }

        // public bool IsOpenedFor(IToken token) => IsOpened && token is HeroToken hero && hero.InventoryManager.Equals(openedManager);
    }
}