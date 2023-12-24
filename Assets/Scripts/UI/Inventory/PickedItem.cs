using System;
using Gameplay.Inventory;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class PickedItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform shadow;

        public void PickItem(Scriptable.Item item)
        {
            if (item is null)
            {
                gameObject.SetActive(false);
                return;
            }

            icon.sprite = item.Sprite;
            gameObject.SetActive(true);
        }

        public void ShowState(bool? state, InventoryManager receiver)
        {
            if (state is null || receiver is null || !receiver.TryGetHero(out HeroToken hero))
            {
                shadow.gameObject.SetActive(false);
                return;
            }
            
            text.color = state.Value ? GlobalDefinitions.TokenOutlineGreenColor : GlobalDefinitions.TokenOutlineRedColor;
            text.SetText(state.Value ? $"Give to {hero.Scriptable.Name}" : $"{hero.Scriptable.Name} is too far");
            shadow.sizeDelta = text.GetPreferredValues();
            shadow.gameObject.SetActive(true);
        }
    }
}