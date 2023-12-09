using Gameplay.Tokens;
using UI;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [Header("BaseAbility Fields")]
        [SerializeField] protected string title;
        [SerializeField] protected Sprite icon;
        [SerializeField, TextArea] protected string detailDescription;
        [SerializeField, TextArea] protected string literalDescription;

        public IToken Caster { get; private set; }
        public AbilitySlot AbilitySlot { get; set; }
        public virtual string Title => title;
        public virtual Sprite Icon => icon;
        public virtual string DetailDescription => detailDescription;
        public virtual string LiteralDescription => literalDescription;


        
        

        protected virtual void OnTokenSet(IToken token) { }
        
        public void SetToken(IToken token)
        {
            Caster = token;
            OnTokenSet(token);
        }
    }
}