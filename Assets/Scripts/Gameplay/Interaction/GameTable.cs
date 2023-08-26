using UnityEngine;

namespace Gameplay.Interaction
{
    public class GameTable : MonoBehaviour, IInteractable
    {
        public bool CanInteract => false;
    }
}