using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Interaction
{
    public class KeyListener : MonoBehaviour
    {
        public delegate void KeyPressEvent(in HashSet<KeyCode> keyCodes);
        public static event KeyPressEvent OnAnyKeyDown;
        public delegate void KeyReleaseEvent(KeyCode keyCode);
        public static event KeyReleaseEvent OnKeyReleased;

        private static readonly HashSet<KeyCode> HeldKeys = new();

        private void Update()
        {
            if(Input.anyKeyDown) OnAnyKeyDown?.Invoke(HeldKeys);

            var released = HeldKeys.Where(Input.GetKeyUp).ToArray();
            HeldKeys.ExceptWith(released);
            foreach (KeyCode keyCode in released) 
                OnKeyReleased?.Invoke(keyCode);
        }
    }
}