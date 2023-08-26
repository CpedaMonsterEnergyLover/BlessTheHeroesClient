﻿using UnityEngine;
using Util.Interface;

namespace Scriptable
{
    public abstract class Item : ScriptableObject, IInventoryItem
    {
        [Header("Item data")] 
        [SerializeField] private new string name;
        [SerializeField] private string description;
        [SerializeField] private int price;
        [SerializeField] private Sprite sprite;

        public string Description => description;
        public int Price => price;
        public string Name => name;
        public Sprite Sprite => sprite;
        
        // IInventoryItem
        public abstract int StackSize { get; }
        public abstract string CategoryName { get; }
        public abstract bool AllowClick { get; }
        public abstract void OnClick();
    }
}