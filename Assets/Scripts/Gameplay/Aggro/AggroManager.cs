using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gameplay.Cards;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Aggro
{
    public abstract class AggroManager<T, TJ> : MonoBehaviour, IAggroManager
    where T : IToken
    where TJ : IToken
    {
        protected TJ Token { get; private set; }
        public IToken Wearer => Token;
        protected Dictionary<T, int> cluster;



#if UNITY_EDITOR
        protected abstract Color GizmosColor { get; }
        protected abstract Vector3 GizmosOffset { get; }
#endif
        protected abstract TJ[] GetAllies(Card card);
        protected abstract T[] GetEnemies(Card card);

        public void Activate(IToken wearer)
        {
            cluster = new Dictionary<T, int>();
            Token = (TJ) wearer;
            OnSelfMove(Token, Token.TokenCard);
            gameObject.SetActive(true);
        }
        
        private void OnDestroy()
        {
            var tmp = cluster.Keys.ToArray();
            foreach (T token in tmp) RemoveAggro(int.MaxValue, token);
            cluster.Clear();
        }

        private void OnSelfMove(IToken t, Card card)
        {
            var tmp = cluster.Keys.ToArray();
            foreach (T token in tmp)
            {
                if(OutOfAggroRange(token.TokenCard)) 
                    RemoveAggro(int.MaxValue, token);
            }
            
            var enemies = GetEnemies(card);
            var allies = GetAllies(card);
            if (enemies.Length == 0) return;

            int initialAggro = allies.Length == 1 ? 2 : 1;
            foreach (T enemy in enemies)
            {
                AddAggro(initialAggro, enemy);
            }
        }

        private void AddAggro(int amount, T source, bool mirrored)
        {
            if (cluster.ContainsKey(source))
                cluster[source] += amount;
            else
            {
                cluster.Add(source, amount);
                if(!mirrored) source.IAggroManager.AddAggro(amount, Token, true);
            }
        }

        private void RemoveAggro(int amount, T source, bool mirrored)
        {
            if(!cluster.ContainsKey(source)) return;

            cluster[source] -= amount;
            if(!mirrored) source.IAggroManager.RemoveAggro(amount, Token, true);
            if (cluster[source] <= 0) 
                cluster.Remove(source);
        }
        
        public void AddAggro(int amount, IToken source, bool mirrored = false)
        {
            if(amount <= 0 || source is not T ss || OutOfAggroRange(source.TokenCard)) return;
            AddAggro(amount, ss, mirrored);
        }
        public void RemoveAggro(int amount, IToken source, bool mirrored = false)
        {
            if(source is T ss) RemoveAggro(amount, ss, mirrored);
        }
        
        private bool OutOfAggroRange(Card target)
        {
            Card current = Token.TokenCard;
            return !current.Equals(target) && (current.GridPosition - target.GridPosition).sqrMagnitude != 1;
        }
        
        protected virtual void OnEnable()
        {
            Token.OnMove += OnSelfMove;
        }

        protected virtual void OnDisable()
        {
            Token.OnMove -= OnSelfMove;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(Token is null) return;
            Vector3 tPos = Token.TokenTransform.position;
            Gizmos.color = GizmosColor;
            foreach (var t in cluster.Keys) 
                Gizmos.DrawLine(tPos + GizmosOffset, t.TokenTransform.position + GizmosOffset);
        }

        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Token.ScriptableToken.Name}'s Aggro Cluster: [");
            foreach (var (t, v) in cluster) sb.Append($" ({t.ScriptableToken.Name} -> {v}) ");
            sb.Append("]");
            Debug.Log(sb);
        }
#endif
    }
}