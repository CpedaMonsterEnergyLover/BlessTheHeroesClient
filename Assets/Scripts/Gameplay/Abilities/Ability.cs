using Gameplay.Tokens;
using UI;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [Header("BaseAbility Fields")] 
        [SerializeField] private new string name;
        [SerializeField] protected Sprite icon;
        [SerializeField, TextArea] private string detailDescription;
        [SerializeField, TextArea] private string literalDescription;

        protected IToken Token { get; private set; }
        public AbilitySlot AbilitySlot { get; set; }
        public string Name => name;
        public virtual Sprite Icon => icon;
        public string DetailDescription => detailDescription;
        public string LiteralDescription => literalDescription;

        

        protected virtual void OnTokenSet(IToken token){ }

        public void SetToken(IToken token)
        {
            Token = token;
            OnTokenSet(token);
        }
    }
}