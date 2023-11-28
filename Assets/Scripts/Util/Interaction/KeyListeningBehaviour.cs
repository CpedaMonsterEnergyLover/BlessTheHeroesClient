using System.Collections.Generic;
using Gameplay.Interaction;
using UnityEngine;

namespace Util.Interaction
{
    public abstract class KeyListeningBehaviour : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            KeyListener.OnAnyKeyDown += OnAnyKeyDown;
            KeyListener.OnKeyReleased += OnKeyReleased;
        }

        protected virtual void OnDisable()
        {
            KeyListener.OnAnyKeyDown -= OnAnyKeyDown;
            KeyListener.OnKeyReleased -= OnKeyReleased;
        }

        protected abstract void OnAnyKeyDown(in HashSet<KeyCode> keyCodes);
        protected abstract void OnKeyReleased(KeyCode keyCode);
    }
}