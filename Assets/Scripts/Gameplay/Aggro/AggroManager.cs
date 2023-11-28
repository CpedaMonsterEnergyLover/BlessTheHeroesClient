using Gameplay.GameField;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;

namespace Gameplay.Aggro
{
    public abstract class AggroManager<T, TJ> : MonoBehaviour, IAggroManager
    where T : IToken
    where TJ : IToken
    {
        [SerializeField] protected TMP_Text aggroText;

        protected T Token { get; private set; }
        public IToken IToken => Token;



        private void Awake() => Token = GetComponentInParent<T>();

        protected virtual void OnEnable()
        {
            Token.OnDeath += OnDeath;
            Token.OnMove += OnMove;
        }

        protected virtual void OnDisable()
        {
            Token.OnDeath -= OnDeath;
            Token.OnMove -= OnMove;
        }


        public void AddAggro(int amount, IToken t)
        {
            if(t is TJ tt) AddAggro(amount, tt);
        }

        public abstract void AddAggro(int amount, TJ source);
        public abstract void ClearAggro();
        protected abstract void OnMove(IToken t, Card card);
        private void OnDeath(IToken _) => ClearAggro();
    }
}