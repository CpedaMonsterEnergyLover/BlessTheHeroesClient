using Gameplay.Tokens;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [Header("BaseAbility Fields")]
        [SerializeField] protected string title;
        [SerializeField] protected Sprite icon;
        [FormerlySerializedAs("detailDescription")] [SerializeField, TextArea] protected string statDescription;
        [FormerlySerializedAs("literalDescription")] [SerializeField, TextArea] protected string description;

        public IToken Caster { get; private set; }
        public AbilitySlot AbilitySlot { get; set; }
        public virtual string Title => title;
        public virtual Sprite Icon => icon;
        public virtual string StatDescription => statDescription;
        public virtual string Description => description;


        
        

        protected virtual void OnTokenSet(IToken token) { }
        
        public void SetToken(IToken token)
        {
            Caster = token;
            OnTokenSet(token);
        }
    }
}