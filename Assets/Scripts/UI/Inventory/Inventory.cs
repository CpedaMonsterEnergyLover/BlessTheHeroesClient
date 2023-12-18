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
        [SerializeField] private TMP_Text titleText;
        
        private InventorySlot[] slots;
        private Tween tween;
        private InventoryManager openedManager;

        private bool lockTarget;
        private bool isOpened;



        public void Toggle(HeroToken hero, bool targetStaysTheSame = false)
        {
            if(tween is not null) return;
            lockTarget = targetStaysTheSame;
            bool state = !slotsTransform.gameObject.activeInHierarchy;
            if(state)
            {
                Open(hero);
                slotsTransform.localScale = Vector3.zero;
                slotsTransform.gameObject.SetActive(true);
            } else Close();
            tween = slotsTransform.DOScale(state ? Vector3.one : Vector3.zero, 0.15f).OnComplete(() =>
            {
                if(!state) slotsTransform.gameObject.SetActive(false);
                tween = null;
            });
        }

        private void Open(HeroToken hero)
        {
            InventoryManager manager = hero.InventoryManager;
            isOpened = true;
            openedManager = manager;
            openedManager.OnCoinsUpdate += UpdateCoinsText;
            openedManager.OnItemsUpdate += UpdateInventory;
            /*if(!lockTarget) */TokenBrowser.OnTokenSelected += OnTokenSelected;
            UpdateHero(hero);
            TryCreateSlots(manager.Capacity);
            UpdateCoinsText(manager.Coins);
            UpdateInventory(manager.Items);
        }

        private void Close()
        {
            if(openedManager is null) return;
            isOpened = false;
            openedManager.OnCoinsUpdate -= UpdateCoinsText;
            openedManager.OnItemsUpdate -= UpdateInventory;
            /*if(!lockTarget) */TokenBrowser.OnTokenSelected -= OnTokenSelected;
            openedManager = null;
        }

        private void UpdateInventory(Item[] items)
        {
            if(!isOpened) return;

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

        private void UpdateHero(HeroToken hero)
        {
            avatarImage.sprite = hero.Scriptable.Sprite;
            titleText.SetText($"{hero.Scriptable.Name}'s inventory");
        }
        
        private void UpdateCoinsText(int coins) => coinsText.SetText($"Coins: {coins}");

        private void TryCreateSlots(int amount)
        {
            if(slots is not null) return;
            
            slots = new InventorySlot[amount];
            for (int i = 0; i < amount; i++) 
                slots[i] = Instantiate(inventorySlotPrefab, slotsTransform);
        }

        private void OnTokenSelected(IToken token)
        {
            Close();
            
            if(token is not HeroToken hero)
            {
                slotsTransform.gameObject.SetActive(false);
                return;
            }

            Open(hero);
        }
    }
}